using System;
using System.Globalization;
using System.Windows.Data;
using MediaBrowser.Model.Session;

namespace Emby.WindowsPhone.Converters
{
    public class NowPlayingProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            var item = value as SessionInfoDto;
            if (item == null)
            {
                return 0;
            }

            var playedRuntime = ((double)item.PlayState.PositionTicks/(double)item.NowPlayingItem.RunTimeTicks)*100;

            return playedRuntime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
