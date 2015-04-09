using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class RoleConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty SelectedPersonProperty =
            DependencyProperty.Register("SelectedPerson", typeof (BaseItemPerson), typeof (RoleConverter), new PropertyMetadata(default(BaseItemPerson)));

        public BaseItemPerson SelectedPerson
        {
            get { return (BaseItemPerson) GetValue(SelectedPersonProperty); }
            set { SetValue(SelectedPersonProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var film = value as BaseItemDto;

            if (film == null || film.People == null)
            {
                return string.Empty;
            }

            var person = film.People.FirstOrDefault(x => x.Name == SelectedPerson.Name);
            return person != null ? person.Role : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
