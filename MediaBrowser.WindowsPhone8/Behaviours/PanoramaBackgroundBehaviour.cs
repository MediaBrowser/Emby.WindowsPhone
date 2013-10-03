using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cimbalino.Phone.Toolkit.Behaviors;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Behaviours
{
    public class PanoramaBackgroundBehaviour : SafeBehavior<Panorama>
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(PanoramaBackgroundBehaviour), new PropertyMetadata(default(string), BackgroundChanged));

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PanoramaBackgroundBehaviour), new PropertyMetadata(default(Stretch), BackgroundChanged));

        public Stretch Stretch
        {
            get { return (Stretch) GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(PanoramaBackgroundBehaviour), new PropertyMetadata(1.0, BackgroundChanged));

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

            var pbb = sender as PanoramaBackgroundBehaviour;
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
