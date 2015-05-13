using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class CanMarkAsWatchedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (BaseItemDto) value;

            switch (item.Type.ToLower())
            {
                case "movie":
                case "video":
                case "series":
                case "season":
                case "episode":
                case "boxset":
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}