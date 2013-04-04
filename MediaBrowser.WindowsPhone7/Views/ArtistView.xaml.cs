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

            Loaded += async (sender, args) =>
                                {
                                    var client = new GZipWebClient();

                                    var item = (DataContext as MusicViewModel).SelectedArtist;

                                    if (item.ProviderIds == null || !item.ProviderIds.ContainsKey("Musicbrainz")) return;

                                    var musicBrainzId = item.ProviderIds["Musicbrainz"];

                                    if (string.IsNullOrEmpty(musicBrainzId)) return;

                                    const string urlFormat = "http://api.fanart.tv/webservice/artist/{0}/{1}/json/artistthumb/1/1/";

                                    var url = string.Format(urlFormat, Constants.FanArtApiKey, musicBrainzId /*"e6de1f3b-6484-491c-88dd-6d619f142abc"*/);

                                    try
                                    {
                                        var json = await client.DownloadStringTaskAsync(url);

                                        if (string.IsNullOrEmpty(json)) return;

                                        var images = JObject.Parse(json);

                                        var artist = images.First;

                                        var artistInfo = artist.First;

                                        var imageItems = (JArray)artistInfo["artistthumb"];

                                        if (!imageItems.Any()) return;

                                        var imageUrl = imageItems[0]["url"].ToString();

                                        if (string.IsNullOrEmpty(imageUrl)) return;

                                        GridForBackground.Background = new ImageBrush
                                                                           {
                                                                               Stretch = Stretch.UniformToFill,
                                                                               Opacity = 0.2,
                                                                               ImageSource = new BitmapImage(new Uri(imageUrl))
                                                                           };
                                    }
                                    catch
                                    {
                                        var v = "";
                                    }
                                };
        }
    }
}