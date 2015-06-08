using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Cimbalino.Toolkit.Converters;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class TitleVisibilityConverter : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Any(x => x == null))
            {
                return Visibility.Visible;
            }

            var hideTitle = (bool)values[0];
            var item = (BaseItemDto) values[1];

            return hideTitle && item.Type != "Episode" ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}