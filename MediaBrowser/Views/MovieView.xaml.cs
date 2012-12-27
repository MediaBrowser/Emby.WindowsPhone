using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MediaBrowser.Model.DTO;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using MediaBrowser.Shared;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for MovieView.
    /// </summary>
    public partial class MovieView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the MovieView class.
        /// </summary>
        public MovieView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var item = (DataContext as MovieViewModel).SelectedMovie;
                MainPanorama.Background = new ImageBrush
                {
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.6,
                    ImageSource = new BitmapImage(new Uri(
                        (string)
                        new Converters.ImageUrlConverter().
                            Convert(item, typeof (string), "backdrop", null)))
                };
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
                DataContext = new MovieViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                                  {
                                      SelectedMovie = (DtoBaseItem)App.SelectedItem
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