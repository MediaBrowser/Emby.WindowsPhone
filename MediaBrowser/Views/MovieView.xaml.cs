using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;

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
    }
}