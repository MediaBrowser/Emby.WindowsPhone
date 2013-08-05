using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Net.NetworkInformation;
using MediaBrowser.Model.Dto;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Model
{
    public class NavigationService : Cimbalino.Phone.Toolkit.Services.NavigationService, INavigationService
    {
        private readonly ILog _logger = new WPLogger(typeof(INavigationService));
        
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
                        NavigateTo("/Views/FolderView.xaml?id=" + item.Id);
                    }
                    else
                    {
                        NavigateTo("/Views/CollectionView.xaml");
                    }
                    break;
                case "folder":
                case "boxset":
                    NavigateTo("/Views/FolderView.xaml?id=" + item.Id);
                    break;
                case "movie":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowMovieMsg));
                    NavigateTo("/Views/MovieView.xaml");
                    break;
                case "series":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowTvSeries));
                    NavigateTo("/Views/TvShowView.xaml");
                    break;
                case "season":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowSeasonMsg));
                    NavigateTo("/Views/SeasonView.xaml");
                    break;
                case "episode":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowEpisodeMsg));
                    NavigateTo("/Views/EpisodeView.xaml");
                    break;
                case "trailer":
                    Messenger.Default.Send(new NotificationMessage(Constants.Messages.ChangeTrailerMsg));
                    NavigateTo("/Views/TrailerView.xaml");
                    break;
                case "musicartist":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicArtistChangedMsg));
                    NavigateTo("/Views/ArtistView.xaml");
                    break;
                case "musicalbum":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.MusicAlbumChangedMsg));
                    NavigateTo("/Views/AlbumView.xaml");
                    break;
            }
        }
    }
}
