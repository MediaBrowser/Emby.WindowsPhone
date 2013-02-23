using System;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var type = value.GetType();
                var apiClient = SimpleIoc.Default.GetInstance<ExtendedApiClient>();
                if (type == typeof (BaseItemDto))
                {
                    
                    var imageType = parameter == null ? string.Empty : (string) parameter;
                    // http://192.168.0.2:8096/mediabrowser/api/image?item.Id=d0aac36ee980d7dc0bcf8323b1884f70&maxheight=173&quality=90
                    var item = (BaseItemDto)value;
                    if (!item.HasPrimaryImage) return "";
                    var imageOptions = new ImageOptions
                                           {
                                               Quality = 90,
                                               MaxHeight = 173,
                                               ImageType = ImageType.Primary
                                           };
                    if (imageType.Equals("logo", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.ImageType = ImageType.Logo;
                    }
                    else if (imageType.Equals("backdrop", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.MaxHeight = 800;
                        imageOptions.ImageType = ImageType.Backdrop;
                    }
                    else if (imageType.Equals("banner", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.MaxHeight = 140;
                        imageOptions.ImageType = ImageType.Banner;
                    }
                    else if (imageType.Equals("art", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.ImageType = ImageType.Art;
                    }
                    else if (imageType.Equals("thumbnail", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.ImageType = ImageType.Thumb;
                    }
                    else if (imageType.Equals("icon", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.MaxHeight = 90;
                    }
                    else if (imageType.Equals("poster", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.MaxHeight = 450;
                    }
                    else if(imageType.Equals("episode", StringComparison.OrdinalIgnoreCase))
                    {
                        imageOptions.MaxHeight = 255;
                    }
                    else
                    {
                        imageOptions.MaxHeight = 200;
                    }
                    if (item.Type == "Series")
                        return apiClient.GetImageUrl(item.Id, imageOptions);
                    return apiClient.GetImageUrl(item, imageOptions);
                }
                else if(type == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson) value;
                    if (person.HasPrimaryImage)
                        return apiClient.GetPersonImageUrl(person, new ImageOptions {MaxWidth = 99, Quality = 90});
                }
                else if(type == typeof(UserDto))
                {
                    var user = (UserDto) value;
                    if (user.HasPrimaryImage)
                    {
                        var url = apiClient.GetUserImageUrl(user, new ImageOptions {MaxHeight = 173, MaxWidth = 173, Quality = 95});
                        return new Uri(url);
                    }
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
