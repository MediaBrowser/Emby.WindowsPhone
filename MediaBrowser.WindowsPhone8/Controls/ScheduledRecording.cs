using System.Windows;
using System.Windows.Controls;
using MediaBrowser.Model.LiveTv;

namespace MediaBrowser.WindowsPhone.Controls
{
    public class ScheduledRecording : Control
    {
        public static readonly DependencyProperty TimerProperty = DependencyProperty.Register(
            "Timer", typeof(ProgramInfoDto), typeof(ScheduledRecording), new PropertyMetadata(default(object), OnTimerChanged));

        public ProgramInfoDto Timer
        {
            get { return (ProgramInfoDto)GetValue(TimerProperty); }
            set { SetValue(TimerProperty, value); }
        }

        private static void OnTimerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sr = sender as ScheduledRecording;
            if (sr == null)
            {
                return;
            }

            var timer = sr.Timer;

            if (!string.IsNullOrEmpty(timer.SeriesTimerId))
            {
                sr.SeriesVisibility = Visibility.Visible;
                sr.ProgrammeVisibility = Visibility.Collapsed;
            }
        }

        public static readonly DependencyProperty SeriesVisibilityProperty = DependencyProperty.Register(
            "SeriesVisibility", typeof (Visibility), typeof (ScheduledRecording), new PropertyMetadata(default(Visibility)));

        public Visibility SeriesVisibility
        {
            get { return (Visibility) GetValue(SeriesVisibilityProperty); }
            set { SetValue(SeriesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ProgrammeVisibilityProperty = DependencyProperty.Register(
            "ProgrammeVisibility", typeof (Visibility), typeof (ScheduledRecording), new PropertyMetadata(default(Visibility)));

        public Visibility ProgrammeVisibility
        {
            get { return (Visibility) GetValue(ProgrammeVisibilityProperty); }
            set { SetValue(ProgrammeVisibilityProperty, value); }
        }

        public ScheduledRecording()
        {
            DefaultStyleKey = typeof (ScheduledRecording);
        }
    }
}
