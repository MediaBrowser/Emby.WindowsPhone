using GalaSoft.MvvmLight.Messaging;

namespace MediaBrowser.WindowsPhone.Messaging
{
    public class NowPlayingMessage : MessageBase
    {
        public NowPlayingMessage(bool startTimer)
        {
            StartTimer = startTimer;
        }

        public bool StartTimer { get; set; }
    }
}