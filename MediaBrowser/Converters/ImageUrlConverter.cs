using System;
using System.Windows.Data;
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
                if (type == typeof (DTOBaseItem))
                {
                    string imageType = parameter == null ? string.Empty : (string) parameter;
                    // http://192.168.0.2:8096/mediabrowser/api/image?item.Id=d0aac36ee980d7dc0bcf8323b1884f70&maxheight=173&quality=90
                    var item = (DTOBaseItem)value;
                    const string baseUrl = "{0}/image?id={1}&maxheight={2}&quality=90&type={3}";
                    if (imageType.Equals("logo", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 173, imageType),
                                       UriKind.Absolute);
                    }
                    else if (imageType.Equals("backdrop", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 800, imageType),
                                       UriKind.Absolute);
                    }
                    else if (imageType.Equals("banner", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 140, imageType),
                                       UriKind.Absolute);
                    }
                    else if (imageType.Equals("art", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 173, imageType),
                                       UriKind.Absolute);
                    }
                    else if (imageType.Equals("thumbnail", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 173, imageType),
                                       UriKind.Absolute);
                    }
                    else if (imageType.Equals("icon", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 75, ""));
                    }
                    else if (imageType.Equals("poster", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 450, ""));
                    }
                    else if(imageType.Equals("episode", StringComparison.OrdinalIgnoreCase))
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 225, ""));
                    }
                    else
                    {
                        return new Uri(string.Format(baseUrl, App.Settings.ApiUrl, item.Id, 173, imageType),
                                       UriKind.Absolute);
                    }
                }
                else if(type == typeof(BaseItemPerson))
                {
                    var person = (BaseItemPerson) value;
                    return new Uri(string.Format("{0}/image?personname={1}&quality=90&maxwidth=99", App.Settings.ApiUrl, person.Name));
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
