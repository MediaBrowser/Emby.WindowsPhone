using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Localisation;

namespace Emby.WindowsPhone.Converters
{
    public class DayPatternConverter : IValueConverter
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

            if (schedule.DayPattern.HasValue)
            {
                switch (schedule.DayPattern.Value)
                {
                    case DayPattern.Daily:
                        return AppResources.LabelScheduleDaily;
                    case DayPattern.Weekdays:
                        return AppResources.LabelScheduleWeekdays;
                    case DayPattern.Weekends:
                        return AppResources.LabelScheduleWeekends;
                }
                return string.Empty;
            }

            var days = string.Join(",", schedule.Days.Select(x => x.GetLocalisedName()));
            return days;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
