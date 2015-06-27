using MediaBrowser.Model;
using System;
using System.Windows.Data;
using Emby.WindowsPhone.Model;

namespace Emby.WindowsPhone.Converters
{
    public class SettingsConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null && parameter != null)
            {
                var settingType = (string) parameter;
                switch(settingType)
                {
                    case "connectiondetails":
                        var connectionDetails = (ConnectionDetails) value;
                        return string.Format("{0}:{1}", connectionDetails.HostName, connectionDetails.PortNo);
                        break;
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
