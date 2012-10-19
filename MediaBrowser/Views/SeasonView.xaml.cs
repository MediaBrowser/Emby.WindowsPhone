using System.Windows.Navigation;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using MediaBrowser.Shared;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for SeasonView.
    /// </summary>
    public partial class SeasonView : PhoneApplicationPage
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
                var item = App.SelectedItem;
                var vm = ViewModelLocator.GetTvViewModel(item.ParentId.Value);
                vm.SelectedSeason = item;
                vm.seasonDataLoaded = false;
                DataContext = vm;
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