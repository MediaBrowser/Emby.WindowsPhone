using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ScottIsAFool.Windows8.Controls
{
    public class ExtendedProgressBar : ProgressBar
    {
        public ExtendedProgressBar()
        {
            DefaultStyleKey = typeof (ExtendedProgressBar);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (ExtendedProgressBar), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof (HorizontalAlignment), typeof (ExtendedProgressBar), new PropertyMetadata(default(HorizontalAlignment)));

        public HorizontalAlignment TextAlignment
        {
            get { return (HorizontalAlignment) GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty FontForegroundProperty =
            DependencyProperty.Register("FontForeground", typeof (Brush), typeof (ExtendedProgressBar), new PropertyMetadata(default(SolidColorBrush)));

        public Brush FontForeground
        {
            get { return (Brush) GetValue(FontForegroundProperty); }
            set { SetValue(FontForegroundProperty, value); }
        }
    }
}
