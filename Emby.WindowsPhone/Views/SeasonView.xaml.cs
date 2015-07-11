using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.ViewModel;

namespace Emby.WindowsPhone.Views
{
    /// <summary>
    /// Description for SeasonView.
    /// </summary>
    public partial class SeasonView
    {
        /// <summary>
        /// Initializes a new instance of the SeasonView class.
        /// </summary>
        public SeasonView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<TvViewModel>(GetType(), false);
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                var item = (BaseItemDto)App.SelectedItem;
                var vm = ViewModelLocator.GetTvViewModel(item.SeriesId);
                DataContext = vm;
                vm.SelectedSeason = item;
                vm.SeasonDataLoaded = false;
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
    }
}