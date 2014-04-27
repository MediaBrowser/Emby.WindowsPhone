using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Behaviors;

namespace MediaBrowser.WindowsPhone.Behaviours
{
    public class ControlBackgroundBehaviour : SafeBehavior<Control>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(ControlBackgroundBehaviour), new PropertyMetadata(default(string), BackgroundChanged));

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ControlBackgroundBehaviour), new PropertyMetadata(default(Stretch), BackgroundChanged));

        public Stretch Stretch
        {
            get { return (Stretch) GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(ControlBackgroundBehaviour), new PropertyMetadata(1.0, BackgroundChanged));

        public double Opacity
        {
            get { return (double) GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        private static void BackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            var pbb = sender as ControlBackgroundBehaviour;
            if (pbb == null || string.IsNullOrEmpty(pbb.Source))
            {
                return;
            }

            var panorama = pbb.AssociatedObject;
            if (panorama == null)
            {
                return;
            }

            panorama.Background = new ImageBrush
            {
                Stretch = pbb.Stretch,
                Opacity = pbb.Opacity,
                ImageSource = new BitmapImage(new Uri(pbb.Source))
            };
        }
    }

    public class PanelBackgroundBehaviour : SafeBehavior<Panel>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(PanelBackgroundBehaviour), new PropertyMetadata(default(string), BackgroundChanged));

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PanelBackgroundBehaviour), new PropertyMetadata(default(Stretch), BackgroundChanged));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(PanelBackgroundBehaviour), new PropertyMetadata(1.0, BackgroundChanged));

        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        private static void BackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            var pbb = sender as PanelBackgroundBehaviour;
            if (pbb == null || string.IsNullOrEmpty(pbb.Source))
            {
                return;
            }

            var panorama = pbb.AssociatedObject;
            if (panorama == null)
            {
                return;
            }

            panorama.Background = new ImageBrush
            {
                Stretch = pbb.Stretch,
                Opacity = pbb.Opacity,
                ImageSource = new BitmapImage(new Uri(pbb.Source))
            };
        }
    }
}
