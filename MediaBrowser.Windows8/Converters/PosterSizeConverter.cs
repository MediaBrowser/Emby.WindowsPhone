using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class PosterSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && parameter != null)
            {
                var property = (string) parameter;
                var detailIsOn = (bool) value;
                var size = detailIsOn ? new Size(110, 150) : new Size(143, 220);
                return property.Equals("width") ? size.Width : size.Height;
            }
            return 120;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
