using System;
using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using MediaBrowser.Model.Dto;


namespace MediaBrowser.WindowsPhone.Model
{
    public class NavigationService : INavigationService
    {
        private PhoneApplicationFrame _mainFrame;

        public event NavigatingCancelEventHandler Navigating;

        public void NavigateTo(Uri pageUri)
        {
            if (EnsureMainFrame())
            {
                _mainFrame.Navigate(pageUri);
            }
        }

        public void NavigateTo(string pageUri)
        {
            NavigateTo(new Uri(pageUri, UriKind.Relative));
        }

        public void GoBack()
        {
            if (EnsureMainFrame()
                && _mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

        private bool EnsureMainFrame()
        {
            if (_mainFrame != null)
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (_mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                return true;
            }
            return false;
        }

        public bool IsNetworkAvailable
        {
            get
            {
                var result = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                if (!result || NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.None)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => App.ShowMessage("", "No network connection available"));
                }
                return result;
            }
        }

        public void NavigateToPage(string link)
        {
            if (!string.IsNullOrEmpty(link))
            {
                if (EnsureMainFrame())
                {
                    var root = Application.Current.RootVisual as PhoneApplicationFrame;
                    root.Navigate(new Uri(link, UriKind.Relative));
                }
            }
        }


        public void NavigateToPage(BaseItemDto item)
        {
            App.SelectedItem = item;
            switch (item.Type.ToLower())
            {
                case "folder":
                case "collectionfolder":
                case "genre":
                case "trailercollectionfolder":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowFolderMsg));
                    NavigateToPage("/Views/CollectionView.xaml");
                    break;
                case "movie":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowMovieMsg));
                    NavigateToPage("/Views/MovieView.xaml");
                    break;
                case "series":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowTvSeries));
                    NavigateToPage("/Views/TvShowView.xaml");
                    break;
                case "season":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowSeasonMsg));
                    NavigateToPage("/Views/SeasonView.xaml");
                    break;
                case "episode":
                    //Messenger.Default.Send(new NotificationMessage(item, Constants.ShowEpisodeMsg));
                    NavigateToPage("/Views/EpisodeView.xaml");
                    break;
                case "trailer":
                    Messenger.Default.Send(new NotificationMessage(Constants.ChangeTrailerMsg));
                    NavigateToPage("/Views/TrailerView.xaml");
                    break;
                case "musicartist":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.MusicArtistChangedMsg));
                    NavigateToPage("/Views/ArtistView.xaml");
                    break;
                case "musicalbum":
                    Messenger.Default.Send(new NotificationMessage(item, Constants.MusicAlbumChangedMsg));
                    NavigateToPage("/Views/AlbumView.xaml");
                    break;
            }
        }
    }
}
