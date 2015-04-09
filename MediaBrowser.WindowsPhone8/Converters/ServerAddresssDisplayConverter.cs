using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.ApiClient;

namespace Emby.WindowsPhone.Converters
{
    public class ServerAddresssDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var server = value as ServerInfo;
            if (server == null) return string.Empty;

            if (string.IsNullOrEmpty(server.RemoteAddress))
            {
                return server.LocalAddress;
            }

            return string.Format("{0},\n{1}", server.LocalAddress, server.RemoteAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
