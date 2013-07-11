using System;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.SilverlightMediaFramework.Core;
using Microsoft.SilverlightMediaFramework.Core.Media;
using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView
    {
        public VideoPlayerView()
        {
            Log = new WPLogger(typeof(VideoPlayerView));

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
            Log.ErrorException("Error playing video: " + e.Value.Message, e.Value);
        }
    }
}