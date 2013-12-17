using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.ViewModel;
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
                DataContext = History.Current.GetLastItem<MovieViewModel>(GetType(), false);
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                DataContext = new MovieViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
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
                Header = "play from"
            };
            playFromMenuItem.Click += (o, args) =>
            {
                
            };

            var playFromOnClientMenuItem = new MenuItem
            {
                Header = "play from on..."
            };
            playFromOnClientMenuItem.Click += (o, args) =>
            {

            };

            contextMenu.Items.Add(playFromMenuItem);
            contextMenu.Items.Add(playFromOnClientMenuItem);

            ContextMenuService.SetContextMenu(item, contextMenu);

            contextMenu.IsOpen = true;
        }
    }
}