using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Streaming;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class LocalisedEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GroupBy)
            {
                var groupBy = (GroupBy) value;
                return groupBy.GetLocalisedName();
            }

            if (value is RecordedGroupBy)
            {
                var recordedGroupBy = (RecordedGroupBy) value;
                return recordedGroupBy.GetLocalisedName();
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BitRateLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lmh = (StreamingLMH) value;
            return lmh.GetBitrateLabel();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ResolutionLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var res = (StreamingResolution) value;
            return res.GetProperResolutionName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
