using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Notifications;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class NotificationLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notificationLevel = (NotificationLevel) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
