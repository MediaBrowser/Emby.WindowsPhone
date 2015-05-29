using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.PlayerFramework;
using SM.Media;

namespace Emby.WindowsPhone.Behaviours
{
    public class VideoStreamBehaviour : Behavior<MediaPlayer>
    {
        private static IMediaStreamFacade _mediaStreamFacade;

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

                var mss = await _mediaStreamFacade.CreateMediaStreamSourceAsync(new Uri(HlsUrl), CancellationToken.None);
                if (mss == null)
                {
                    Debug.WriteLine("MainPage.PlayCurrentTrackAsync() Unable to create media stream source");
                    return;
                }

                AssociatedObject.SetSource(mss);
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
            if (_mediaStreamFacade != null)
                return;

            _mediaStreamFacade = MediaStreamFacadeSettings.Parameters.Create();
        }
    }
}