using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;

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
                    ImageSource =
                        new BitmapImage(
                        (Uri)
                        new Converters.ImageUrlConverter().Convert(item,
                                                                    typeof (
                                                                        Uri),
                                                                    "backdrop",
                                                                    null))
                };
            };
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage(Constants.ClearFilmAndTvMsg));
            }
        }
    }
}