using System.Collections.Generic;
using System.Linq;
using System;
using MediaBrowser.Model.Entities;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class VideoResolutionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                if (value is List<MediaStream>)
                {
                    var videoInfo = ((List<MediaStream>) value).FirstOrDefault(x => x.Type == MediaStreamType.Video);
                    if (videoInfo != default(MediaStream))
                    {
                        if ((videoInfo.Width >= 1280 && videoInfo.Width < 1920) ||
                            videoInfo.Height == 720) // 720p
                        {
                            return "720";
                        }
                        else if (videoInfo.Width >= 1920)
                        {
                            return "1080";
                        }
                        else if (videoInfo.Width <= 640)
                        {
                            return "480";
                        }
                    }
                }
            }
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
