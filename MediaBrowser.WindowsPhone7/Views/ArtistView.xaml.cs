using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpGIS;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for ArtistView.
    /// </summary>
    public partial class ArtistView : PhoneApplicationPage
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