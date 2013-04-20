using System;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.SilverlightMediaFramework.Core;
using Microsoft.SilverlightMediaFramework.Core.Media;
using Microsoft.SilverlightMediaFramework.Plugins.Primitives;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView : PhoneApplicationPage
    {
        private readonly ILog _logger;

        public VideoPlayerView()
        {
            _logger = new WPLogger(typeof(VideoPlayerView));

            InitializeComponent();
            Loaded += (sender, args) =>
                          {
                              var url = SimpleIoc.Default.GetInstance<VideoPlayerViewModel>().VideoUrl;
                              thePlayer.Playlist.Add(new PlaylistItem
                              {
                                  DeliveryMethod = DeliveryMethods.NotSpecified,
                                  MediaSource = new Uri(url)
                              });
                              thePlayer.Play();
                          };

            
        }

        private void ThePlayer_OnMediaFailed(object sender, CustomEventArgs<Exception> e)
        {
            _logger.Log("Error playing video: " + e.Value.Message, LogLevel.Error);
            _logger.Log(e.Value.StackTrace, LogLevel.Error);
        }
    }
}