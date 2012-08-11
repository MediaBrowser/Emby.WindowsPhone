using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
                              var item = (DataContext as TvViewModel).SelectedTvSeries.Item;
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
    }
}