using GalaSoft.MvvmLight.Messaging;

namespace Emby.WindowsPhone.Messaging
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