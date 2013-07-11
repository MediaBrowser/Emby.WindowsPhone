using System.Windows;
using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for EpisodeView.
    /// </summary>
    public partial class EpisodeView
    {
        /// <summary>
        /// Initializes a new instance of the EpisodeView class.
        /// </summary>
        public EpisodeView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                var item = (BaseItemDto)App.SelectedItem;
                var vm = ViewModelLocator.GetTvViewModel(item.SeriesId);
                vm.SelectedEpisode = item;
                DataContext = vm;
            }
        }

        private void GestureListenerDragStarted(object sender, DragStartedGestureEventArgs e)
        {
            var vm = (TvViewModel) DataContext;
            if (vm.Episodes.Count <= 1) return;
            // initialize the drag
            //var fe = sender as FrameworkElement;
            //fe.SetHorizontalOffset(0); 
        }

        private void GestureListenerDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            // handle the drag to offset the element
            var fe = sender as FrameworkElement;
            double offset = fe.GetHorizontalOffset().Value + e.HorizontalChange;
            fe.SetHorizontalOffset(offset);
        }

        private void GestureListenerDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            
        }
    }
}