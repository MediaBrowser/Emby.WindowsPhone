using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.ViewModel;
using MediaBrowser.WindowsPhone.ViewModel.Playlists;
using Microsoft.Phone.Net.NetworkInformation;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Model
{
    public class NavigationService : Cimbalino.Phone.Toolkit.Services.NavigationService, INavigationService
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
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowFolderMsg));
                    if (App.SpecificSettings.JustShowFolderView)
                    {
                        NavigateTo(Constants.Pages.FolderView + item.Id);
                    }
                    else
                    {
                        NavigateTo(Constants.Pages.CollectionView);
                    }
                    break;
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
                        Messenger.Default.Send(new NotificationMessage(Constants.Messages.ChangeTrailerMsg));
                    NavigateTo(Constants.Pages.TrailerView);
                    break;
                case "musicartist":
                case "artist":
                    if (SimpleIoc.Default.GetInstance<MusicViewModel>() != null)
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicArtistChangedMsg));
                    NavigateTo(Constants.Pages.ArtistView);
                    break;
                case "musicalbum":
                    if (SimpleIoc.Default.GetInstance<MusicViewModel>() != null)
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicAlbumChangedMsg));
                    NavigateTo(Constants.Pages.AlbumView);
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
                default:
                    if (SimpleIoc.Default.GetInstance<GenericItemViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.GenericItemChangedMsg));
                    }

                    NavigateTo(Constants.Pages.GenericItemView);
                    break;
            }
        }
    }
}
