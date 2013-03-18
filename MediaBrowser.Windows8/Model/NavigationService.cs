using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Windows8.Views;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml.Controls;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.Windows8.Model
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        public Frame Frame { set { _frame = value; } }

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        [PreferredConstructor]
        public NavigationService()
        {

        }

        public void Navigate(Type pageType, object parameter = null)
        {
            if (_frame != null)
                _frame.Navigate(pageType, parameter);
        }

        public bool CanGoBack
        {
            get { return _frame != null && _frame.CanGoBack; }
        }

        public void GoBack()
        {
            // Use the navigation frame to return to the previous page
            if (CanGoBack) _frame.GoBack();
        }

        public bool CanGoForward
        {
            get { return _frame != null && _frame.CanGoForward; }
        }

        public void GoForward()
        {
            // Use the navigation frame to return to the previous page
            if (CanGoForward) _frame.GoForward();
        }

        public void Navigate<T>(object parameter = null)
        {
            var type = typeof(T);
            Navigate(type, parameter);
        }

        public bool IsNetworkAvailable
        {
            get
            {
                var iConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
                if (iConnectionProfile == null)
                {
                    return false;
                }

                var connectionProfileInfo = iConnectionProfile.GetNetworkConnectivityLevel();
                ConnectivityLevel = connectionProfileInfo;
                return ConnectivityLevel != NetworkConnectivityLevel.None;
            }
        }

        public void NavigateToPage(object item)
        {
            if (item is BaseItemDto)
            {
                var type = ((BaseItemDto)item).Type.ToLower();
                if (type.Contains("collectionfolder")) type = "collectionfolder";

                switch (type)
                {
                    case "folder":
                    case "collectionfolder":
                    case "genre":
                    case "trailercollectionfolder":
                        //Navigate<CollectionView>(item);
                        Navigate<FolderView>(item);
                        break;
                    case "movie":
                        Navigate<MovieView>(item);
                        break;
                    case "series":
                        Navigate<TvSeriesView>(item);
                        break;
                    case "season":
                        Navigate<SeasonView>(item);
                        break;
                    case "episode":
                        Navigate<EpisodeView>(item);
                        break;
                    case "trailer":
                        Messenger.Default.Send(new NotificationMessage(item, Constants.ChangeTrailerMsg));
                        Navigate<TrailerView>();
                        break;
                    case "musicartist":
                        Messenger.Default.Send(new NotificationMessage(item, Constants.MusicArtistChangedMsg));
                        Navigate<ArtistView>();
                        break;
                    case "musicalbum":
                        Messenger.Default.Send(new NotificationMessage(item, Constants.MusicAlbumChangedMsg));
                        Navigate<AlbumView>();
                        break;
                    default:
                        break;
                }
            }
            else if (item is BaseItemPerson)
            {
                Navigate<FolderView>(item);
            }
        }

        public void PlayVideoItem(BaseItemDto item, bool isResume)
        {
            Messenger.Default.Send(new NotificationMessage(item, isResume, Constants.PlayVideoItemMsg));
            Navigate<VideoPlayer>();
        }

        public NetworkConnectivityLevel ConnectivityLevel { get; protected set; }
    }
}
