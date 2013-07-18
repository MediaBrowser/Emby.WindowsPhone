using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using MediaBrowser.Model.Notifications;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class NotificationLevelToBrushConverter : IValueConverter
    {
        private readonly Color _information = Colors.Blue;
        private readonly Color _warning = Colors.Orange;
        private readonly Color _error = Colors.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var notificationLevel = (NotificationLevel) value;

            switch (notificationLevel)
            {
                case NotificationLevel.Error:
                    return new SolidColorBrush(_error);
                case NotificationLevel.Normal:
                    return new SolidColorBrush(_information);
                case NotificationLevel.Warning:
                    return new SolidColorBrush(_warning);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
