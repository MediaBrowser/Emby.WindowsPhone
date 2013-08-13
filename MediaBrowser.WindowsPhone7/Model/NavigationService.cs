using System.Windows;
using GalaSoft.MvvmLight.Messaging;
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
        
        public void NavigateTo(BaseItemDto item)
        {
            App.SelectedItem = item;
            var type = item.Type.ToLower();
            if (type.Contains("collectionfolder")) type = "collectionfolder";
            switch (type)
            {
                case "collectionfolder":
                case "genre":
                case "trailercollectionfolder":
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
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowMovieMsg));
                    NavigateTo(Constants.Pages.MovieView);
                    break;
                case "series":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowTvSeries));
                    NavigateTo(Constants.Pages.TvShowView);
                    break;
                case "season":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowSeasonMsg));
                    NavigateTo(Constants.Pages.SeasonView);
                    break;
                case "episode":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowEpisodeMsg));
                    NavigateTo(Constants.Pages.EpisodeView);
                    break;
                case "trailer":
                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.ChangeTrailerMsg));
                    NavigateTo(Constants.Pages.TrailerView);
                    break;
                case "musicartist":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicArtistChangedMsg));
                    NavigateTo(Constants.Pages.ArtistView);
                    break;
                case "musicalbum":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicAlbumChangedMsg));
                    NavigateTo(Constants.Pages.AlbumView);
                    break;
            }
        }
    }
}
