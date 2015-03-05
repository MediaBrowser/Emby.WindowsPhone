using System;
using System.Globalization;
using Cimbalino.Toolkit.Converters;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RemoteClientMultiConverter : MultiValueConverterBase
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return string.Empty;
            }

            var deviceName = (string) values[0];
            var clientType = (string) values[1];

            return string.Format("{0} - {1}", clientType, deviceName);
        }

        public override object[] ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
