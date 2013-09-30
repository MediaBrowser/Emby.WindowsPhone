using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaBrowser.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for ArtistView.
    /// </summary>
    public partial class ArtistView
    {
        /// <summary>
        /// Initializes a new instance of the ArtistView class.
        /// </summary>
        public ArtistView()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
                          {
                              var item = (DataContext as MusicViewModel).SelectedArtist;

                              var url = (string)
                                        new Converters.ImageUrlConverter().
                                            Convert(item, typeof (string), "primary", null);

                              if (!string.IsNullOrEmpty(url))
                              {
                                  MainPivot.Background = new ImageBrush
                                                             {
                                                                 Stretch = Stretch.UniformToFill,
                                                                 Opacity = 0.2,
                                                                 ImageSource = new BitmapImage(new Uri(url))
                                                             };
                              }
                          };
        }
    }
}