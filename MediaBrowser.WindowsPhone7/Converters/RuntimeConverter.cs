using System;
using System.Windows.Data;

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
                        return string.Format("{0}m", ts.Minutes);
                    }
                    return string.Format("{0}h {1}m", ts.Hours, ts.Minutes);
                }
                else if (displayType == "audio")
                {
                    if (ts.Hours == 0)
                    {
                        return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                    }
                    return string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
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
