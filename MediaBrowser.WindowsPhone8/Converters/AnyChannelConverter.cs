using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.LiveTv;
using Emby.WindowsPhone.Localisation;

namespace Emby.WindowsPhone.Converters
{
    public class AnyChannelConverter : IValueConverter
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

            return schedule.RecordAnyChannel ? AppResources.LabelScheduleAnyChannel : schedule.ChannelName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}