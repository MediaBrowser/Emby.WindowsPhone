using System;
using System.Windows.Data;
using Emby.WindowsPhone.Localisation;

namespace MediaBrowser.WindowsPhone.Converters
{
    public class RuntimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null)
            {
                var displayType = "normal";
                if (parameter != null) displayType = (string) parameter;
                var ticks = long.Parse(value.ToString());
                var ts = TimeSpan.FromTicks(ticks);
                if (displayType == "normal")
                {
                    if (ts.Hours == 0)
                    {
                        return string.Format("{0}{1}", ts.Minutes, AppResources.LabelReallyShortMinutes);
                    }
                    return string.Format("{0}{1} {2}{3}", ts.Hours, AppResources.LabelReallyShortHours, ts.Minutes, AppResources.LabelReallyShortMinutes);
                }

                if (displayType == "audio")
                {
                    if (ts.Hours == 0)
                    {
                        return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                    }
                    return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                }

                if (displayType == "livetv")
                {
                    return string.Format("{0}{1}", (int)ts.TotalMinutes, AppResources.LabelReallyShortMinutes);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
