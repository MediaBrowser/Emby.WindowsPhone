using System.Windows;
using Cimbalino.Toolkit.Behaviors;
using MediaBrowser.Model.Sync;

namespace Emby.WindowsPhone.Behaviours
{
    public class SyncAppBarButton : ApplicationBarIconButton
    {
        public static readonly DependencyProperty IsRemoveProperty = DependencyProperty.Register(
            "IsRemove", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool IsRemove
        {
            get { return (bool) GetValue(IsRemoveProperty); }
            set { SetValue(IsRemoveProperty, value); }
        }

        public static readonly DependencyProperty SyncPolicyProperty = DependencyProperty.Register(
            "SyncPolicy", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool SyncPolicy
        {
            get { return (bool) GetValue(SyncPolicyProperty); }
            set { SetValue(SyncPolicyProperty, value); }
        }
        
        public static readonly DependencyProperty SyncStatusProperty = DependencyProperty.Register(
            "SyncStatus", typeof(SyncJobItemStatus), typeof(SyncAppBarButton), new PropertyMetadata(default(SyncJobItemStatus), OnChanged));

        public SyncJobItemStatus SyncStatus
        {
            get { return (SyncJobItemStatus)GetValue(SyncStatusProperty); }
            set { SetValue(SyncStatusProperty, value); }
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
            bool isVisible;
            if (IsRemove)
            {
                isVisible = SyncStatus == SyncJobItemStatus.Synced && SyncPolicy;
            }
            else
            {
                isVisible = SyncStatus != SyncJobItemStatus.Synced && SyncPolicy;
            }

            IsVisible = isVisible;
        }
    }
}
