using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.SilverlightMediaFramework.Core;
using Microsoft.SilverlightMediaFramework.Core.Media;
using Microsoft.SilverlightMediaFramework.Plugins.Primitives;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class VideoPlayerView : PhoneApplicationPage
    {
        public VideoPlayerView()
        {
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
            var s = "";
        }
    }
}