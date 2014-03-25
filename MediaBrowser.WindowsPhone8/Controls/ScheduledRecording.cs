using System.Windows;
using System.Windows.Controls;
using MediaBrowser.Model.LiveTv;

namespace MediaBrowser.WindowsPhone.Controls
{
    public class ScheduledRecording : Control
    {
        public static readonly DependencyProperty SeriesTimerIdProperty = DependencyProperty.Register(
            "SeriesTimerId", typeof(string), typeof(ScheduledRecording), new PropertyMetadata(default(string), OnTimerChanged));

        public string SeriesTimerId
        {
            get { return (string)GetValue(SeriesTimerIdProperty); }
            set { SetValue(SeriesTimerIdProperty, value); }
        }

        public static readonly DependencyProperty TimerIdProperty = DependencyProperty.Register(
            "TimerId", typeof(string), typeof(ScheduledRecording), new PropertyMetadata(default(string), OnTimerChanged));

        public string TimerId
        {
            get { return (string)GetValue(TimerIdProperty); }
            set { SetValue(TimerIdProperty, value); }
        }

        private static void OnTimerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sr = sender as ScheduledRecording;
            if (sr == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(sr.SeriesTimerId))
            {
                sr.SeriesVisibility = Visibility.Visible;
                sr.ProgrammeVisibility = Visibility.Collapsed;
            }
            else if (!string.IsNullOrEmpty(sr.TimerId) && string.IsNullOrEmpty(sr.SeriesTimerId))
            {
                sr.SeriesVisibility = Visibility.Collapsed;
                sr.ProgrammeVisibility = Visibility.Visible;
            }
        }

        public static readonly DependencyProperty SeriesVisibilityProperty = DependencyProperty.Register(
            "SeriesVisibility", typeof(Visibility), typeof(ScheduledRecording), new PropertyMetadata(default(Visibility)));

        public Visibility SeriesVisibility
        {
            get { return (Visibility)GetValue(SeriesVisibilityProperty); }
            set { SetValue(SeriesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ProgrammeVisibilityProperty = DependencyProperty.Register(
            "ProgrammeVisibility", typeof(Visibility), typeof(ScheduledRecording), new PropertyMetadata(default(Visibility)));

        public Visibility ProgrammeVisibility
        {
            get { return (Visibility)GetValue(ProgrammeVisibilityProperty); }
            set { SetValue(ProgrammeVisibilityProperty, value); }
        }

        public ScheduledRecording()
        {
            DefaultStyleKey = typeof(ScheduledRecording);
        }
    }
}
