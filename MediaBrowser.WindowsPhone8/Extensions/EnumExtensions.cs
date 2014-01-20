using System;
using System.Globalization;

namespace MediaBrowser.WindowsPhone.Extensions
{
    public static class EnumExtensions
    {
        public static string GetLocalisedDay(this DayOfWeek dayOfWeek)
        {
            return DateTimeFormatInfo.CurrentInfo == null ? dayOfWeek.ToString() : DateTimeFormatInfo.CurrentInfo.GetDayName(dayOfWeek);
        }
    }
}
