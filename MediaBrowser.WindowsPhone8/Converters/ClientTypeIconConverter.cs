using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Session;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class ClientTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "/Images/Logo.png";
            }

            var clientInfo = value as SessionInfoDto;
            if (clientInfo == null)
            {
                return "/Images/Logo.png";
            }

            switch (clientInfo.Client.ToLower())
            {
                case "dashboard":
                    var device = clientInfo.DeviceName.ToLower();

                    string imageUrl;

                    if (device.IndexOf("chrome", StringComparison.Ordinal) != -1)
                    {
                        imageUrl = "chrome";
                    }
                    else if (device.IndexOf("firefox", StringComparison.Ordinal) != -1)
                    {
                        imageUrl = "firefox";
                    }
                    else if (device.IndexOf("internet explorer", StringComparison.Ordinal) != -1)
                    {
                        imageUrl = "ie";
                    }
                    else if (device.IndexOf("safari", StringComparison.Ordinal) != -1)
                    {
                        imageUrl = "safari";
                    }
                    else
                    {
                        imageUrl = "html5";
                    }

                    return string.Format("/Images/Remote/Clients/{0}.png", imageUrl);
                case "mb-classic":
                    return "/Images/Remote/Clients/mbc.png";
                case "media browser theater":
                    return "/Images/Remote/Clients/mb.png";
                case "android":
                    return "/Images/Remote/Clients/android.png";
                case "roku":
                    return "/Images/Remote/Clients/roku.png";
                case "ios":
                    return "/Images/Remote/Clients/ios.png";
                case "windows rt":
                    return "/Images/Remote/Clients/windowsrt.png";
                case "windows phone":
                    return "/Images/Remote/Clients/windowsphone.png";
                case "dlna":
                    return "/Images/Remote/Clients/dlna.png";
                case "mbkinect":
                    return "/Images/Remote/Clients/mbkinect.png";
                default:
                    return "/Images/Logo.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
