using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class VirtualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var item = value as BaseItemDto;
            if (item == null)
            {
                return false;
            }

            return item.LocationType != LocationType.Virtual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VirtualVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }

            var item = value as BaseItemDto;
            if (item == null)
            {
                return Visibility.Visible;
            }

            return item.LocationType == LocationType.Virtual ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
