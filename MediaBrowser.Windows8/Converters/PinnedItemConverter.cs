using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class PinnedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isPinned = (bool)value;

            return isPinned ? (Style)Application.Current.Resources["PinAppBarButtonStyle"] : (Style)Application.Current.Resources["UnPinAppBarButtonStyle"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
