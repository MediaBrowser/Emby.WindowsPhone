namespace MediaBrowser.WindowsPhone.Model.Streaming
{
    public class TranscodeSetting360p : TranscodeSetting
    {
        public TranscodeSetting360p()
        {
            Height = 480;
            Width = 640;
            VideoBitrate = 1000000;
            AudioBitrate = 128000;
            AudioChannels = 2;
        }
    }
}