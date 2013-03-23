using System;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class TileHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var type = value.ToString();
            if (type.ToLower().Contains("collection") || type.ToLower().Contains("folder"))
            {
                return 35;
            }
            return 65;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
