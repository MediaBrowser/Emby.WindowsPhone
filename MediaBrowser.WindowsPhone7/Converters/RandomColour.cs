using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RandomColour: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var r = new Random();
            var i = r.Next(0, Constants.TileColours.Length);
            var colour = Constants.TileColours[i];

            var red = byte.Parse(colour.Substring(0, 2), NumberStyles.HexNumber);
            var green = byte.Parse(colour.Substring(2, 2), NumberStyles.HexNumber);
            var blue = byte.Parse(colour.Substring(4, 2), NumberStyles.HexNumber);

            var returnColour = Color.FromArgb(255, red, green, blue);

            return new SolidColorBrush(returnColour);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
