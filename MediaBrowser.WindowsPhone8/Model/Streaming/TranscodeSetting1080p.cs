namespace MediaBrowser.WindowsPhone.Model.Streaming
{
    public class TranscodeSetting1080p : TranscodeSetting
    {
        public TranscodeSetting1080p(int bitrate, int audioBitrate, int audioChannels)
        {
            Height = 1080;
            Width = 1920;
            VideoBitrate = bitrate;
            AudioBitrate = audioBitrate;
            AudioChannels = audioChannels;
        }
    }
}