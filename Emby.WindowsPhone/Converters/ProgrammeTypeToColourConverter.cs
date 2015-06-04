using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MediaBrowser.Model.Dto;

namespace Emby.WindowsPhone.Converters
{
    public class ProgrammeTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            var programme = value as BaseItemDto;
            if (programme == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            if (programme.IsKids ?? false)
            {
                return Application.Current.Resources["ChildProgrammeBrush"];
            }

            if (programme.IsSports ?? false)
            {
                return Application.Current.Resources["SportsProgrammeBrush"];
            }

            if (programme.IsNews ?? false)
            {
                return Application.Current.Resources["NewsProgrammeBrush"];
            }

            if (programme.IsMovie ?? false)
            {
                return Application.Current.Resources["MovieProgrammeBrush"];
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
