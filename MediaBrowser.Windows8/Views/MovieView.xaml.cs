using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Windows8.ViewModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.Shared;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MovieView : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public MovieView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) =>
                          {
                              var id = ((MovieViewModel) DataContext).SelectedMovie.Id;
                              Messenger.Default.Send(new NotificationMessage(id, Constants.MovieViewLoadedMsg));
                          };
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
                var item = (BaseItemDto) e.Parameter;
                DataContext = new MovieViewModel(ViewModelLocator.ApiClient, ViewModelLocator.NavigationService)
                {
                    SelectedMovie = item
                };
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);
            }
        }

        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            ActionControls.Visibility = viewState == ApplicationViewState.Snapped ? Visibility.Collapsed : Visibility.Visible;
            return base.DetermineVisualState(viewState);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().ItemClicked.Execute(e);
        }
    }
}
