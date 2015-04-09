using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MediaBrowser.Model.LiveTv;

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

            var programme = value as ProgramInfoDto;
            if (programme == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            if (programme.IsKids)
            {
                return Application.Current.Resources["ChildProgrammeBrush"];
            }

            if (programme.IsSports)
            {
                return Application.Current.Resources["SportsProgrammeBrush"];
            }

            if (programme.IsNews)
            {
                return Application.Current.Resources["NewsProgrammeBrush"];
            }

            if (programme.IsMovie)
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
