using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Services;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class LiveTvInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            var tvInfo = value as LiveTvInfo;
            if (tvInfo == null || tvInfo.ActiveServiceName.IsNullOrEmpty())
            {
                return Visibility.Collapsed;
            }

            var allowedUser = tvInfo.EnabledUsers.FirstOrDefault(x => x == AuthenticationService.Current.LoggedInUserId);

            return allowedUser != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
