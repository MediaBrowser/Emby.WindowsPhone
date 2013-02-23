using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MediaBrowser.Windows8.Converters
{
    public class TextLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value != null)
            {
                double extra = 0;
                if (parameter != null)
                    extra = System.Convert.ToDouble(parameter.ToString());
                var text = (string) value;
                var tb = new TextBlock
                             {
                                 Text = text,
                                 Style = (Style) Application.Current.Resources["GroupHeaderTextStyle"]
                             };
                tb.Measure(new Size(double.MaxValue, double.MaxValue));
                return tb.DesiredSize.Width + extra;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
