using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.ViewModel;
using Emby.WindowsPhone.ViewModel.Playlists;
using Microsoft.Phone.Net.NetworkInformation;

namespace Emby.WindowsPhone.Services
{
    public class NavigationService : Cimbalino.Toolkit.Services.NavigationService, INavigationService
    {
        public bool IsNetworkAvailable
        {
            get
            {
                var result = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                if (!result || NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => App.ShowMessage("No network connection available"));
                }
                return result;
            }
        }

        public bool IsOnWifi
        {
            get
            {
                var networkType = NetworkInterface.NetworkInterfaceType;
                return networkType.ToString().Contains("Ethernet")
                       || networkType == NetworkInterfaceType.Wireless80211;
            }
        }

        public void NavigateTo(string uri)
        {
            Navigate(uri);
        }

        public void NavigateTo(string uri, bool clearBackStack)
        {
            var extra = clearBackStack ? "?clearbackstack=true" : string.Empty;
            NavigateTo(uri + extra);
        }

        public void NavigateTo(BaseItemInfo item)
        {
            var dto = new BaseItemDto
            {
                Id = item.Id,
                Type = item.Type,
                Name = item.Name
            };

            NavigateTo(dto);
        }
        
        public void NavigateTo(BaseItemDto item)
        {
            App.SelectedItem = item;
            var type = item.Type.ToLower();
            if (type.Contains("collectionfolder")) type = "collectionfolder";
            if (type.StartsWith("genre")) type = "genre";
            switch (type)
            {
                case "collectionfolder":
                case "genre":
                case "trailercollectionfolder":
                case "playlistsfolder":
                case "userview":
                    if (App.SpecificSettings.JustShowFolderView)
                    {
                        NavigateTo(Constants.Pages.FolderView + item.Id);
                    }
                    else
                    {
                        NavigateTo(Constants.Pages.CollectionView);
                    }
                    break;
                case "photoalbum":
                case "folder":
                case "boxset":
                    NavigateTo(Constants.Pages.FolderView + item.Id);
                    break;
                case "movie":
                    NavigateTo(Constants.Pages.MovieView);
                    break;
                case "series":
                    NavigateTo(Constants.Pages.TvShowView);
                    break;
                case "season":
                    NavigateTo(Constants.Pages.SeasonView);
                    break;
                case "episode":
                    NavigateTo(Constants.Pages.EpisodeView);
                    break;
                case "trailer":
                    if (SimpleIoc.Default.GetInstance<TrailerViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.ChangeTrailerMsg));
                        NavigateTo(Constants.Pages.TrailerView);
                    }
                    break;
                case "musicartist":
                case "artist":
                    if (SimpleIoc.Default.GetInstance<MusicViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicArtistChangedMsg));
                        NavigateTo(Constants.Pages.ArtistView);
                    }
                    break;
                case "musicalbum":
                    if (SimpleIoc.Default.GetInstance<MusicViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicAlbumChangedMsg));
                        NavigateTo(Constants.Pages.AlbumView);
                    }
                    break;
                case "channel":
                case "channelfolderitem":
                    NavigateTo(Constants.Pages.Channels.ChannelView + item.Id);
                    break;
                case "playlist":
                    if (SimpleIoc.Default.GetInstance<ServerPlaylistsViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ServerPlaylistChangedMsg));
                        NavigateTo(Constants.Pages.Playlists.PlaylistView);
                    }
                        
                    break;
                case "person":
                    var actor = new BaseItemPerson
                    {
                        Name = item.Name,
                        Id = item.Id,
                        PrimaryImageTag = item.HasPrimaryImage ? item.ImageTags.FirstOrDefault(x => x.Key == ImageType.Primary).Value : string.Empty
                    };

                    App.SelectedItem = actor;
                    NavigateTo(Constants.Pages.ActorView);
                    break;
                default:
                    if (SimpleIoc.Default.GetInstance<GenericItemViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.GenericItemChangedMsg));
                        NavigateTo(Constants.Pages.GenericItemView);
                    }

                    break;
            }
        }
    }
}
