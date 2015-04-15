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

        public static readonly DependencyProperty HasSyncJobProperty = DependencyProperty.Register(
            "HasSyncJob", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool HasSyncJob
        {
            get { return (bool) GetValue(HasSyncJobProperty); }
            set { SetValue(HasSyncJobProperty, value); }
        }

        public static readonly DependencyProperty IsSyncedProperty = DependencyProperty.Register(
            "IsSynced", typeof (bool), typeof (SyncAppBarButton), new PropertyMetadata(default(bool), OnChanged));

        public bool IsSynced
        {
            get { return (bool) GetValue(IsSyncedProperty); }
            set { SetValue(IsSyncedProperty, value); }
        }

        public static readonly DependencyProperty SyncStatusProperty = DependencyProperty.Register(
            "SyncStatus", typeof (SyncJobStatus), typeof (SyncAppBarButton), new PropertyMetadata(default(SyncJobStatus)));

        public SyncJobStatus SyncStatus
        {
            get { return (SyncJobStatus) GetValue(SyncStatusProperty); }
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
                isVisible = IsSynced && SyncPolicy;
            }
            else
            {
                isVisible = !HasSyncJob && SyncPolicy && !IsSynced;
            }

            IsVisible = isVisible;
        }
    }
}
