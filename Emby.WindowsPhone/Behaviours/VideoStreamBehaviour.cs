using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.PlayerFramework;
using ScottIsAFool.WindowsPhone.Logging;
using SM.Media;

namespace Emby.WindowsPhone.Behaviours
{
    public class VideoStreamBehaviour : Behavior<MediaPlayer>
    {
        private static IMediaStreamFacade _mediaStreamFacade;
        private static ILog _logger;

        public VideoStreamBehaviour()
        {
            _logger = new WPLogger(GetType());
        }

        public static readonly DependencyProperty VideoStreamProperty = DependencyProperty.Register(
            "VideoStream", typeof(IsolatedStorageFileStream), typeof(VideoStreamBehaviour), new PropertyMetadata(default(Stream), OnVideoStreamChanged));

        public IsolatedStorageFileStream VideoStream
        {
            get { return (IsolatedStorageFileStream)GetValue(VideoStreamProperty); }
            set { SetValue(VideoStreamProperty, value); }
        }

        public static readonly DependencyProperty HlsUrlProperty = DependencyProperty.Register(
            "HlsUrl", typeof (string), typeof (VideoStreamBehaviour), new PropertyMetadata(default(string), OnVideoStreamChanged));

        public string HlsUrl
        {
            get { return (string) GetValue(HlsUrlProperty); }
            set { SetValue(HlsUrlProperty, value); }
        }

        private static void OnVideoStreamChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behaviour = sender as VideoStreamBehaviour;
            if (behaviour != null)
            {
                behaviour.SetStream();
            }
        }

        private async void SetStream()
        {
            if (!string.IsNullOrEmpty(HlsUrl))
            {
                InitialiseMediaStream();

                try
                {
                    _logger.Info("Creating MediaStreamSource");
                    var mss = await _mediaStreamFacade.CreateMediaStreamSourceAsync(new Uri(HlsUrl), CancellationToken.None);
                    if (mss == null)
                    {
                        _logger.Info("Unable to create media stream source");
                        return;
                    }

                    _logger.Info("Delaying...");
                    await Task.Delay(5);

                    _logger.Info("Setting stream source to video");
                    AssociatedObject.SetSource(mss);
                }
                catch (TaskCanceledException tEx)
                {
                    _logger.ErrorException("SetStream()", tEx);
                }
            }
            else
            {
                if (VideoStream != null && VideoStream.CanRead)
                {
                    AssociatedObject.SetSource(VideoStream);
                }
            }
        }

        private static void InitialiseMediaStream()
        {
            _logger.Info("Creating stream facade");
            _mediaStreamFacade = MediaStreamFacadeSettings.Parameters.Create();
            
        }
    }
}