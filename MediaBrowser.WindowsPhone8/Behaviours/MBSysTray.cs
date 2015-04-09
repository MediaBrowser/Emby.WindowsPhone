using System.Windows;
using System.Windows.Media;
using ScottIsAFool.WindowsPhone.Behaviours;

namespace Emby.WindowsPhone.Behaviours
{
    public class MBSysTray : SystemTrayProgressIndicatorBehaviour
    {
        public MBSysTray()
        {
            DotColor = ((SolidColorBrush) Application.Current.Resources["PhoneAccentBrush"]).Color;
        }
    }
}
