using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class EpisodeNumberVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var p = "";
            if (parameter == null) p = "margin";
            else p = parameter.ToString();
            if (value is string)
            {
                if(p.Equals("visibility"))
                    return value.ToString().ToLower().Equals("episode") ? Visibility.Visible : Visibility.Collapsed;
                
                return value.ToString().ToLower().Equals("episode") ? new Thickness(0, 0, 6, 0) : new Thickness(0);
            }
            if (p.Equals("visibility"))
                return Visibility.Collapsed;
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
