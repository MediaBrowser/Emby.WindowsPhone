namespace Emby.WindowsPhone.Model.Streaming
{
    public class TranscodeSetting720p : TranscodeSetting
    {
        public TranscodeSetting720p(int bitrate, int audioBitrate, int audioChannels)
        {
            Height = 720;
            Width = 1280;
            VideoBitrate = bitrate;
            AudioBitrate = audioBitrate;
            AudioChannels = audioChannels;
        }
    }
}