namespace Emby.WindowsPhone.Model.Streaming
{
    public class TranscodeSetting480p : TranscodeSetting
    {
        public TranscodeSetting480p(int bitrate, int audioBitrate)
        {
            Height = 640;
            Width = 720;
            VideoBitrate = bitrate;
            AudioBitrate = audioBitrate;
            AudioChannels = 2;
        }
    }
}