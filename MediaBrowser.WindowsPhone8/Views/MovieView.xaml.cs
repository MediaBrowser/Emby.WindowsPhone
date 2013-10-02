using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.ViewModel;

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
    }
}