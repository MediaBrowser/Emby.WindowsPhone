using System.Windows;
using Cimbalino.Toolkit.Behaviors;

namespace MediaBrowser.WindowsPhone.Behaviours
{
    public class SyncAppBarButton : ApplicationBarIconButton
    {
        public bool IsRemove { get; set; }

        public static readonly DependencyProperty SyncPolicyProperty = DependencyProperty.Register(
            "SyncPolicy", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool SyncPolicy
        {
            get { return (bool) GetValue(SyncPolicyProperty); }
            set { SetValue(SyncPolicyProperty, value); }
        }

        public static readonly DependencyProperty HasSyncJobProperty = DependencyProperty.Register(
            "HasSyncJob", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool HasSyncJob
        {
            get { return (bool) GetValue(HasSyncJobProperty); }
            set { SetValue(HasSyncJobProperty, value); }
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as SyncAppBarButton;
            if (button != null)
            {
                button.SetIsVisible();
            }
        }

        private void SetIsVisible()
        {
            var isVisible = (!HasSyncJob || IsRemove) && SyncPolicy;
            IsVisible = isVisible;
        }
    }
}
