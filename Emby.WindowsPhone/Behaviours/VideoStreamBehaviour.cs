using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.PlayerFramework;

namespace Emby.WindowsPhone.Behaviours
{
    public class VideoStreamBehaviour : Behavior<MediaPlayer>
    {
        public static readonly DependencyProperty VideoStreamProperty = DependencyProperty.Register(
            "VideoStream", typeof(IsolatedStorageFileStream), typeof(VideoStreamBehaviour), new PropertyMetadata(default(Stream), OnVideoStreamChanged));

        public IsolatedStorageFileStream VideoStream
        {
            get { return (IsolatedStorageFileStream)GetValue(VideoStreamProperty); }
            set { SetValue(VideoStreamProperty, value); }
        }

        private static void OnVideoStreamChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behaviour = sender as VideoStreamBehaviour;
            if (behaviour != null)
            {
                behaviour.SetStream();
            }
        }

        private void SetStream()
        {
            if (VideoStream != null && VideoStream.CanRead)
            {
                AssociatedObject.SetSource(VideoStream);
            }
        }
    }
}