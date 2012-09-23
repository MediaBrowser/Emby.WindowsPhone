using System;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.DTO;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Type type = value.GetType();
                var apiClient = SimpleIoc.Default.GetInstance<ApiClient>();
                if (type == typeof (DtoBaseItem))
                {
                    string imageType = parameter == null ? string.Empty : (string) parameter;
                    // http://192.168.0.2:8096/mediabrowser/api/image?item.Id=d0aac36ee980d7dc0bcf8323b1884f70&maxheight=173&quality=90
                    var item = (DtoBaseItem)value;
                    if (imageType.Equals("logo", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Logo, maxHeight: 173, quality: 90);
                    }
                    else if (imageType.Equals("backdrop", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Backdrop, maxHeight: 800, quality: 90);
                    }
                    else if (imageType.Equals("banner", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Banner, maxHeight: 140, quality: 90);
                    }
                    else if (imageType.Equals("art", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Art, maxHeight: 173, quality: 90);
                    }
                    else if (imageType.Equals("thumbnail", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Thumbnail, maxHeight: 173, quality: 90);
                    }
                    else if (imageType.Equals("icon", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Primary, maxHeight: 75, quality: 90);
                    }
                    else if (imageType.Equals("poster", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Primary, maxHeight: 450, quality: 90);
                    }
                    else if(imageType.Equals("episode", StringComparison.OrdinalIgnoreCase))
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Primary, maxHeight: 225, quality: 90);
                    }
                    else
                    {
                        return apiClient.GetImageUrl(item.Id, ImageType.Primary, maxHeight: 200, quality: 90);
                    }
                }
                else if(type == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson) value;
                    return apiClient.GetPersonImageUrl(person.Name, maxWidth: 99, quality: 90);
                }
                else if(type == typeof(DtoUser))
                {
                    var user = (DtoUser) value;
                    var url = apiClient.GetUserImageUrl(user.Id, maxHeight: 173, maxWidth: 173, quality: 95);
                    return new Uri(url);
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
