using MediaBrowser.Model.Dto;
using System;
using System.IO;
using MediaBrowser.Model.Entities;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class VideoFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var item = (BaseItemDto)value;

                if (!string.IsNullOrEmpty(item.Path))
                {
                    switch (item.VideoType)
                    {
                        case VideoType.VideoFile:
                            return Path.GetExtension(item.Path).Replace(".", "").ToUpper();
                        case VideoType.Iso:

                            break;
                        case VideoType.Dvd:
                            break;
                        case VideoType.BluRay:
                            break;
                    }
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
