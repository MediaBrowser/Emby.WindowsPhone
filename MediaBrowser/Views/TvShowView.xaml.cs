using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.DTO;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using MediaBrowser.Shared;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for TvShowView.
    /// </summary>
    public partial class TvShowView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the TvShowView class.
        /// </summary>
        public TvShowView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                var item = (DataContext as TvViewModel).SelectedTvSeries;
                MainPanorama.Background = new ImageBrush
                {
                    Stretch = Stretch.UniformToFill,
                    Opacity = 0.6,
                    ImageSource = new BitmapImage(new Uri(
                        (string)
                        new Converters.ImageUrlConverter().
                            Convert(item, typeof(string), "backdrop", null)))
                };
            };
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
                var item = (DtoBaseItem)App.SelectedItem;
                var vm = ViewModelLocator.GetTvViewModel(item.Id);
                vm.SelectedTvSeries = item;
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