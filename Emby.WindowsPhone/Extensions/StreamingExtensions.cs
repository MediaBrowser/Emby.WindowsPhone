using System;
using Emby.WindowsPhone.Model.Streaming;

namespace Emby.WindowsPhone.Extensions
{
    public static class StreamingExtensions
    {
        public static TranscodeSetting GetSettings(this StreamingQuality quality)
        {
            switch (quality)
            {
                case StreamingQuality.ThreeSixty:
                    return new TranscodeSetting360p();
                case StreamingQuality.FourEightyLow:
                    return new TranscodeSetting480p(620000, 96000);
                case StreamingQuality.FourEightyMedium:
                    return new TranscodeSetting480p(1500000, 196000);
                case StreamingQuality.FourEightyHigh:
                    return new TranscodeSetting480p(4500000, 256000);
                case StreamingQuality.SevenTwentyLow:
                    return new TranscodeSetting720p(5000000, 128000, 2);
                case StreamingQuality.SevenTwentyMedium:
                    return new TranscodeSetting720p(7500000, 256000, 3);
                case StreamingQuality.SevenTwentyHigh:
                    return new TranscodeSetting720p(10000000, 380000, 3);
                case StreamingQuality.TenEightyLow:
                    return new TranscodeSetting1080p(7500000, 196000, 2);
                case StreamingQuality.TenEightyMedium:
                    return new TranscodeSetting1080p(10000000, 256000, 3);
                case StreamingQuality.TenEightyHigh:
                    return new TranscodeSetting1080p(15000000, 750000, 6);
                default:
                    return new TranscodeSettingCopy();
            }
        }

        public static StreamingQuality ToStreamingQuality(this StreamingResolution resolution, StreamingLMH lmh)
        {
            var quality = string.Format("{0}{1}", resolution, lmh);
            if (resolution == StreamingResolution.ThreeSixty)
            {
                return StreamingQuality.ThreeSixty;
            }

            return (StreamingQuality) Enum.Parse(typeof (StreamingQuality), quality, true);
        }

        public static void BreakDown(this StreamingQuality quality, out StreamingResolution resolution, out StreamingLMH lmh)
        {
            switch (quality)
            {
                case StreamingQuality.ThreeSixty:
                    lmh = StreamingLMH.Low;
                    resolution = StreamingResolution.ThreeSixty;
                    break;
                case StreamingQuality.FourEightyLow:
                    lmh = StreamingLMH.Low;
                    resolution = StreamingResolution.FourEighty;
                    break;
                case StreamingQuality.FourEightyMedium:
                    lmh = StreamingLMH.Medium;
                    resolution = StreamingResolution.FourEighty;
                    break;
                case StreamingQuality.FourEightyHigh:
                    lmh = StreamingLMH.High;
                    resolution = StreamingResolution.FourEighty;
                    break;
                case StreamingQuality.SevenTwentyLow:
                    lmh = StreamingLMH.Low;
                    resolution = StreamingResolution.SevenTwenty;
                    break;
                case StreamingQuality.SevenTwentyMedium:
                    lmh = StreamingLMH.Medium;
                    resolution = StreamingResolution.SevenTwenty;
                    break;
                case StreamingQuality.SevenTwentyHigh:
                    lmh = StreamingLMH.High;
                    resolution = StreamingResolution.SevenTwenty;
                    break;
                case StreamingQuality.TenEightyLow:
                    lmh = StreamingLMH.Low;
                    resolution = StreamingResolution.TenEighty;
                    break;
                case StreamingQuality.TenEightyMedium:
                    lmh = StreamingLMH.Medium;
                    resolution = StreamingResolution.TenEighty;
                    break;
                case StreamingQuality.TenEightyHigh:
                    lmh = StreamingLMH.High;
                    resolution = StreamingResolution.TenEighty;
                    break;
                default:
                    lmh = StreamingLMH.Low;
                    resolution = StreamingResolution.ThreeSixty;
                    break;
            }
        }
    }
}
