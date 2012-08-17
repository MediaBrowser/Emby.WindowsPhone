using System;
using System.Windows;
using System.Windows.Navigation;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.DTO;

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


        public void NavigateTopage(BaseItemContainer<ApiBaseItem> item)
        {
            switch (item.Type.ToLower())
            {
                case "folder":
                    if (((ViewModelLocator)Application.Current.Resources["Locator"]).Folder != null)
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(item.Item, Constants.ShowFolderMsg));
                    NavigateToPage("/Views/FolderView.xaml");
                    break;
                case "movie":
                    if(((ViewModelLocator)Application.Current.Resources["Locator"]).Movie != null)
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(item, Constants.ShowMovieMsg));
                    NavigateToPage("/Views/MovieView.xaml");
                    break;
                case "series":
                    if(((ViewModelLocator)Application.Current.Resources["Locator"]).Tv != null)
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(item, Constants.ShowTvSeries));
                    NavigateToPage("/Views/TvShowView.xaml");
                    break;
                case "season":
                    if(((ViewModelLocator)Application.Current.Resources["Locator"]).Tv != null)
                        Messenger.Default.Send<NotificationMessage>(new NotificationMessage(item, Constants.ShowSeasonMsg));
                    NavigateToPage("/Views/SeasonView.xaml");
                    break;
                case "episode":

                    break;
                default:
                    break;
            }
        }
    }
}
