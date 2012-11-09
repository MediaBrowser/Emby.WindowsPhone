using System.Windows.Navigation;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for EpisodeView.
    /// </summary>
    public partial class EpisodeView : PhoneApplicationPage
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
                var item = App.SelectedItem;
                var vm = ViewModelLocator.GetTvViewModel(item.SeriesId.Value);
                vm.SelectedEpisode = item;
                DataContext = vm;
            }
        }
    }
}