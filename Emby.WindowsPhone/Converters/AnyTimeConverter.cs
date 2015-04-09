using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;
using Emby.WindowsPhone.Localisation;

namespace Emby.WindowsPhone.Converters
{
    public class AnyTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var schedule = value as SeriesTimerInfoDto;
            if (schedule == null)
            {
                return string.Empty;
            }

            return schedule.RecordAnyTime ? AppResources.LabelScheduleAnyTime : schedule.StartDate.ToLocalTime().ToShortTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}