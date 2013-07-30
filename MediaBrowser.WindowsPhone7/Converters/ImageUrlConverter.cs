using System;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Search;
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
                if (type == typeof(BaseItemDto))
                {

                    var imageType = parameter == null ? string.Empty : (string)parameter;
                    // http://192.168.0.2:8096/mediabrowser/api/image?item.Id=d0aac36ee980d7dc0bcf8323b1884f70&maxheight=173&quality=90
                    var item = (BaseItemDto)value;
                    return GetDtoImage(item, imageType, apiClient);
                }
                
                if (type == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson)value;
                    if (person.HasPrimaryImage)
                    {
                        return apiClient.GetPersonImageUrl(person, new ImageOptions { MaxWidth = 99, Quality = 90 });
                    }
                }
                
                if (type == typeof(UserDto))
                {
                    var user = (UserDto)value;
                    if (user.HasPrimaryImage)
                    {
                        var url = apiClient.GetUserImageUrl(user, new ImageOptions { MaxHeight = 173, MaxWidth = 173, Quality = 95 });
                        return new Uri(url);
                    }
                }

                if (type == typeof (SearchHint))
                {
                    var searchHint = (SearchHint) value;
                    var imageOptions = new ImageOptions
                    {
#if WP8
                        MaxHeight = 159,
                        MaxWidth = 159
#else
                        MaxHeight = 100,
                        MaxWidth = 100
#endif
                    };

                    switch (searchHint.Type)
                    {
                        case "Person":
                            return apiClient.GetPersonImageUrl(searchHint.Name, imageOptions);
                        case "Artist":
                            return apiClient.GetArtistImageUrl(searchHint.Name, imageOptions);
                        case "MusicGenre":
                        case "GameGenre":
                            return apiClient.GetGenreImageUrl(searchHint.Name, imageOptions);
                        case "Studio":
                            return apiClient.GetStudioImageUrl(searchHint.Name, imageOptions);
                        default:
                            return apiClient.GetImageUrl(searchHint.ItemId, imageOptions);
                    }
                }
            }
            return "";
        }

        private static object GetDtoImage(BaseItemDto item, string imageType, ExtendedApiClient apiClient)
        {
            if (!item.HasPrimaryImage) return "";
            var imageOptions = new ImageOptions
            {
                Quality = 90,
#if WP8
                MaxHeight = 336,
#else
                                               MaxHeight = 173,
#endif
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
            else if (imageType.Equals("backdropsmall", StringComparison.OrdinalIgnoreCase))
            {
#if WP8
                imageOptions.MaxHeight = 336;
#else
                        imageOptions.MaxHeight = 173;
#endif
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
#if WP8
                imageOptions.MaxHeight = 159;
#else
                        imageOptions.MaxHeight = 90;
#endif
            }
            else if (imageType.Equals("poster", StringComparison.OrdinalIgnoreCase))
            {
#if WP8
                imageOptions.MaxHeight = 675;
#else
                        imageOptions.MaxHeight = 450;
#endif
            }
            else if (imageType.Equals("episode", StringComparison.OrdinalIgnoreCase))
            {
#if WP8
                imageOptions.MaxHeight = 382;
#else
                        imageOptions.MaxHeight = 255;
#endif
            }
            else
            {
#if WP8
                imageOptions.MaxHeight = 300;
#else
                        imageOptions.MaxHeight = 200;
#endif
            }
            try
            {
                string url = item.Type == "Series" ? apiClient.GetImageUrl(item.Id, imageOptions) : apiClient.GetImageUrl(item, imageOptions);
                return url;
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
