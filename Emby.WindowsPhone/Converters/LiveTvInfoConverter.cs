using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Services;

namespace Emby.WindowsPhone.Converters
{
    public class LiveTvInfoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var tvInfo = value as LiveTvInfo;
            if (tvInfo == null)
            {
                return false;
            }

            return tvInfo.UserCanHasLiveTv(AuthenticationService.Current.LoggedInUserId);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
