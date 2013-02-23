using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.Dto;
using System;
using MediaBrowser.Model.Entities;
using MediaBrowser.Windows8.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        private static ExtendedApiClient _apiClient;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                _apiClient = SimpleIoc.Default.GetInstance<ExtendedApiClient>();
                var imageOptions = new ImageOptions
                                       {
                                           Quality = 95
                                       };
                if (value.GetType() == typeof(BaseItemDto))
                {
                    var item = (BaseItemDto) value;
                    if (!item.HasPrimaryImage) return "";
                    var parameterString = "";
                    if (parameter != null)
                        parameterString = ((string)parameter).ToLower();
                    
                    return GetItemUrl(item, parameterString, imageOptions);
                }
                if (value.GetType() == typeof(UserDto))
                {
                    var user = (UserDto) value;
                    if (user.HasPrimaryImage)
                    {
                        imageOptions.MaxWidth = 350;
                        imageOptions.MaxHeight = 350;
                        var url = _apiClient.GetUserImageUrl(user, imageOptions);
                        return new Uri(url);
                    }
                    return new Uri("ms-appx:///Assets/UserLoginDefault.png");
                }
                if(value.GetType() == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson) value;
                    if (person.HasPrimaryImage)
                    {
                        imageOptions.MaxHeight = 200;
                        imageOptions.MaxWidth = 100;
                        var url = _apiClient.GetPersonImageUrl(person, imageOptions);
                        return new Uri(url);
                    }
                    return new Uri("ms-appx:///Assets/default_avatar.png");
                }
            }
            return null;
        }

        private static Uri GetItemUrl(BaseItemDto item, string parameterString, ImageOptions imageOptions)
        {
            if(item.Type.ToLower().Contains("folder") && !item.HasPrimaryImage)
                return new Uri("ms-appx:///Assets/Clapperboard.png");
            var type = ImageType.Primary;
            int? maxWidth = 350, maxHeight = 350, randomNumber = null;
            if (parameterString.Equals("smallposter"))
            {

            }
            if (parameterString.Equals("fullposter"))
            {
                maxWidth = 400;
                maxHeight = null;
            }
            if (parameterString.Equals("bigbackdrop"))
            {
                type = ImageType.Backdrop;
                maxWidth = null;
                maxHeight = null;
                // Check a setting to see if random backdrops should be used.
                // TODO: For now, it will always be random
                if (item.BackdropCount > 1)
                {
                    var r = new Random(DateTime.Now.Second);
                    randomNumber = r.Next(0, item.BackdropCount - 1);
                }
            }
            if (parameterString.Equals("snappedbackdrop"))
            {
                type = ImageType.Primary;
                maxWidth = null;
                maxHeight = System.Convert.ToInt32(Window.Current.Bounds.Height);
            }

            imageOptions.MaxWidth = maxWidth;
            imageOptions.MaxHeight = maxHeight;
            imageOptions.ImageIndex = randomNumber;
            imageOptions.ImageType = type;
            string url = _apiClient.GetImageUrl(item, imageOptions);
            return new Uri(url);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
