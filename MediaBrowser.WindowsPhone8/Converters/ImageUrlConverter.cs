using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Search;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var type = value.GetType();
                var apiClient = SimpleIoc.Default.GetInstance<IExtendedApiClient>();
                if (type == typeof(BaseItemDto))
                {

                    var imageType = parameter == null ? string.Empty : (string) parameter;
                    // http://192.168.0.2:8096/mediabrowser/api/image?item.Id=d0aac36ee980d7dc0bcf8323b1884f70&maxheight=173&quality=90
                    var item = (BaseItemDto)value;
                    return GetDtoImage(item, imageType, apiClient);
                }
                
                if (type == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson)value;
                    if (person.HasPrimaryImage)
                    {
                        var smallImageSize = parameter == null;
                        return apiClient.GetPersonImageUrl(person, new ImageOptions
                        {
                            MaxWidth = smallImageSize ? 99 : 200, 
                            Quality = 90,
                            EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers
                        });
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
                        EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers,
                        MaxHeight = 159,
                        MaxWidth = 159
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
                if (type == typeof (BaseItemInfo))
                {
                    var item = (BaseItemInfo) value;
                    var imageType = parameter == null ? string.Empty : (string)parameter;
                    var imageOptions = new ImageOptions
                    {
                        EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers,
                        ImageType = ImageType.Primary
                    };

                    if (imageType.Equals("backdrop"))
                    {
                        imageOptions.ImageType = ImageType.Backdrop;
                        imageOptions.MaxWidth = 480;
                    }
                    else
                    {
                        imageOptions.MaxHeight = 440;
                    }

                    return apiClient.GetImageUrl(item.Id, imageOptions);
                }
                if (type == typeof (ChannelInfoDto))
                {
                    var item = (ChannelInfoDto) value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        MaxHeight = 122
                    };

                    return item.HasPrimaryImage ? apiClient.GetImageUrl(item, imageOptions) : string.Empty;
                }

                if (type == typeof (RecordingInfoDto))
                {
                    var item = (RecordingInfoDto) value;

                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        MaxHeight = 250
                    };

                    return item.HasPrimaryImage ? apiClient.GetImageUrl(item, imageOptions) : string.Empty;
                }

                if (type == typeof (TimerInfoDto))
                {
                    var item = (TimerInfoDto) value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        MaxHeight = 250
                    };

                    return item.ProgramInfo.HasPrimaryImage ? apiClient.GetImageUrl(item.ProgramInfo, imageOptions) : string.Empty;
                }

                if (type == typeof (ProgramInfoDto))
                {
                    var item = (ProgramInfoDto) value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        MaxHeight = 250
                    };

                    return item.HasPrimaryImage ? apiClient.GetImageUrl(item, imageOptions) : string.Empty;
                }
            }

            return "";
        }

        private static object GetDtoImage(BaseItemDto item, string imageType, IExtendedApiClient apiClient)
        {
            if (item.ImageTags.IsNullOrEmpty() && item.BackdropImageTags.IsNullOrEmpty())
            {
                return "";
            }

            var imageOptions = new ImageOptions
            {
                EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers,
                Quality = 90,
                MaxHeight = 336,
                ImageType = ImageType.Primary
            };
            if (imageType.Equals("logo", StringComparison.OrdinalIgnoreCase))
            {
                imageOptions.ImageType = ImageType.Logo;
            }
            else if (imageType.Equals("backdrop"))
            {
                imageOptions.MaxHeight = 800;
                imageOptions.ImageType = ImageType.Backdrop;

                var images = apiClient.GetBackdropImageUrls(item, imageOptions);
                if (!images.IsNullOrEmpty())
                {
                    return images.FirstOrDefault();
                }
            }
            else if (imageType.Equals("primaryorbackdrop"))
            {
                if (!item.HasPrimaryImage)
                {
                    imageOptions.MaxHeight = 800;
                    imageOptions.ImageType = ImageType.Backdrop; 

                    var images = apiClient.GetBackdropImageUrls(item, imageOptions);
                    if (!images.IsNullOrEmpty())
                    {
                        return images.FirstOrDefault();
                    }
                }
            }
            else if (imageType.Equals("backdropsmall", StringComparison.OrdinalIgnoreCase))
            {
                imageOptions.MaxHeight = 336;
                imageOptions.ImageType = ImageType.Backdrop;

                var images = apiClient.GetBackdropImageUrls(item, imageOptions);
                if (!images.IsNullOrEmpty())
                {
                    return images.FirstOrDefault();
                }
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
                imageOptions.MaxHeight = 159;
            }
            else if (imageType.Equals("poster", StringComparison.OrdinalIgnoreCase))
            {
                imageOptions.MaxHeight = 675;
            }
            else if (imageType.Equals("episode", StringComparison.OrdinalIgnoreCase))
            {
                imageOptions.MaxHeight = 382;
            }
            else
            {
                imageOptions.MaxHeight = 300;
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
