using System;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class LikeButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var like = (bool?) value;
            var isLikeButton = ((string) parameter).Equals("like");
            if (like.HasValue)
            {
                if (isLikeButton)
                {
                    return !like.Value;
                }
                return like.Value;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
