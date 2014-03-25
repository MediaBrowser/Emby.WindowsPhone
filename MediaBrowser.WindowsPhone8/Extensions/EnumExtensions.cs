using System;
using System.Globalization;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class EnumExtensions
    {
        public static string GetLocalisedName(this DayOfWeek dayOfWeek)
        {
            return DateTimeFormatInfo.CurrentInfo == null ? dayOfWeek.ToString() : DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
        }

        public static string GetLocalisedName(this GroupBy groupBy)
        {
            switch (groupBy)
            {
                case GroupBy.Genre:
                    return AppResources.Genre;
                case GroupBy.Name:
                    return AppResources.NameLabel;
                case GroupBy.ProductionYear:
                    return AppResources.ProductionYear;
            }

            return groupBy.ToString();
        }

        public static string GetLocalisedName(this RecordedGroupBy recordedGroupBy)
        {
            switch (recordedGroupBy)
            {
                case RecordedGroupBy.Channel:
                    return AppResources.LabelChannel;
                case RecordedGroupBy.RecordedDate:
                    return AppResources.LabelRecordedDate;
                case RecordedGroupBy.ShowName:
                    return AppResources.LabelShowName;
            }

            return recordedGroupBy.ToString();
        }

        public static string GetLocalisedName<T>(this Enum<T> item)
        {
            return item.ToString();
        }
    }
}
