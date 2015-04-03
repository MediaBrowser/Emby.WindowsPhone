using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.WindowsPhone.Messaging;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.WindowsPhone.ViewModel;
using MediaBrowser.WindowsPhone.ViewModel.Remote;
using Microsoft.Phone.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for MovieView.
    /// </summary>
    public partial class MovieView
    {
        /// <summary>
        /// Initializes a new instance of the MovieView class.
        /// </summary>
        public MovieView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<MovieViewModel>(GetType());
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                DataContext = new MovieViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ConnectionManager)
                                  {
                                      SelectedMovie = (BaseItemDto)App.SelectedItem
                                  };
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);
            }
        }

        private void ChapterItem_OnTap(object sender, GestureEventArgs e)
        {
            var item = sender as Grid;
            if (item == null) return;

            var chapterInfo = item.DataContext as ChapterInfoDto;
            if (chapterInfo == null) return;

            var contextMenu = ContextMenuService.GetContextMenu(item);

            if (contextMenu == null)
            {
                contextMenu = new ContextMenu();
            }
            else
            {
                contextMenu.Items.Clear();
            }

            var playFromMenuItem = new MenuItem
            {
                Header = AppResources.MenuPlayFrom
            };

            playFromMenuItem.Click += (o, args) =>
            {
                var vm = (DataContext as MovieViewModel);
                if (vm != null)
                {
                    if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null && vm.SelectedMovie.LocationType != LocationType.Virtual)
                    {
                        Messenger.Default.Send(new VideoMessage(vm.SelectedMovie, true, chapterInfo.StartPositionTicks));
                        NavigationService.Navigate(new Uri(string.Format(Constants.Pages.VideoPlayerView, vm.SelectedMovie.Id, vm.SelectedMovie.Type), UriKind.Relative));
                    }
                }
            };
            contextMenu.Items.Add(playFromMenuItem);

            var playFromOnClientMenuItem = new MenuItem
            {
                Header = AppResources.MenuPlayFromOn
            };
            playFromOnClientMenuItem.Click += (o, args) =>
            {
                var vm = (DataContext as MovieViewModel);
                if (vm != null)
                {
                    if (SimpleIoc.Default.GetInstance<RemoteViewModel>() != null && vm.SelectedMovie.LocationType != LocationType.Virtual)
                    {
                        Messenger.Default.Send(new RemoteMessage(vm.SelectedMovie.Id, chapterInfo.StartPositionTicks));
                        NavigationService.Navigate(new Uri(Constants.Pages.Remote.ChooseClientView, UriKind.Relative));
                    }
                }
            };

            contextMenu.Items.Add(playFromOnClientMenuItem);

            ContextMenuService.SetContextMenu(item, contextMenu);

            contextMenu.IsOpen = true;
        }

    }
}