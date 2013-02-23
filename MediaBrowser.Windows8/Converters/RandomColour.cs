using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MediaBrowser.Windows8.Converters
{
    public class RandomColour: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Random r = new Random();
            int i = r.Next(0, Constants.TileColours.Length);
            string colour = Constants.TileColours[i];

            byte red = byte.Parse(colour.Substring(0, 2), NumberStyles.HexNumber);
            byte green = byte.Parse(colour.Substring(2, 2), NumberStyles.HexNumber);
            byte blue = byte.Parse(colour.Substring(4, 2), NumberStyles.HexNumber);

            Color returnColour = Color.FromArgb(255, red, green, blue);

            return new SolidColorBrush(returnColour);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
