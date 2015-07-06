using System;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Search;
using MediaBrowser.Model.Sync;

namespace Emby.WindowsPhone.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                var type = value.GetType();
                var manager = SimpleIoc.Default.GetInstance<IConnectionManager>();
                var apiClient = manager.GetApiClient(App.ServerInfo.Id);
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
                            Quality = Constants.ImageQuality,
                            EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers
                        });
                    }
                }
                
                if (type == typeof(UserDto))
                {
                    var user = (UserDto)value;
                    if (user.HasPrimaryImage)
                    {
                        var url = apiClient.GetUserImageUrl(user, new ImageOptions { MaxHeight = 250, MaxWidth = 250, Quality = Constants.ImageQuality });
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
                        MaxWidth = 159,
                        Quality = Constants.ImageQuality
                    };

                    switch (searchHint.Type)
                    {
                        case "MusicGenre":
                        case "GameGenre":
                            return apiClient.GetGenreImageUrl(searchHint.Name, imageOptions);
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
                        ImageType = ImageType.Primary,
                        Quality = Constants.ImageQuality
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
                        MaxHeight = 122,
                        Quality = Constants.ImageQuality
                    };

                    return item.HasPrimaryImage ? apiClient.GetImageUrl(item, imageOptions) : string.Empty;
                }
                if (type == typeof(TimerInfoDto))
                {
                    var item = (TimerInfoDto)value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        Quality = Constants.ImageQuality,
                        MaxHeight = 250
                    };

                    return item.ProgramInfo.HasPrimaryImage ? apiClient.GetImageUrl(item.ProgramInfo, imageOptions) : string.Empty;
                }
                
                if (type == typeof (SyncJob))
                {
                    var item = (SyncJob) value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        Quality = Constants.ImageQuality,
                        MaxHeight = 250,
                        Tag = item.PrimaryImageTag
                    };

                    return string.IsNullOrEmpty(item.PrimaryImageItemId) ? string.Empty : apiClient.GetImageUrl(item.PrimaryImageItemId, imageOptions);
                }

                if (type == typeof(SyncJobItem))
                {
                    var item = (SyncJobItem)value;
                    var imageOptions = new ImageOptions
                    {
                        ImageType = ImageType.Primary,
                        Quality = Constants.ImageQuality,
                        MaxHeight = 250,
                        Tag = item.PrimaryImageTag
                    };

                    return string.IsNullOrEmpty(item.PrimaryImageItemId) ? string.Empty : apiClient.GetImageUrl(item.PrimaryImageItemId, imageOptions);
                }
            }

            return "";
        }

        private static object GetDtoImage(BaseItemDto item, string imageType, IApiClient apiClient)
        {
            if (item.ImageTags.IsNullOrEmpty() && item.BackdropImageTags.IsNullOrEmpty())
            {
                return "";
            }

            var imageOptions = new ImageOptions
            {
                EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers,
                Quality = Constants.ImageQuality,
                MaxHeight = 336,
                ImageType = ImageType.Primary
            };

            if (item.Type == "Recording")
            {
                imageOptions.MaxHeight = 250;
            }
            else if (item.Type == "Program")
            {
                imageOptions.MaxHeight = 250;
            }
            else if (imageType.Equals("logo", StringComparison.OrdinalIgnoreCase))
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
                if (!item.ImageTags.ContainsKey(imageOptions.ImageType))
                {
                    return string.Empty;
                }

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
