using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class LikeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //ms-appx:///Assets/UserLoginDefault.png
            var likes = (bool?)value;
            if (likes.HasValue)
            {
                return likes.Value ? new Uri("ms-appx:///Assets/ThumbsUp.png") : new Uri("ms-appx:///Assets/ThumbsDown.png");
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
