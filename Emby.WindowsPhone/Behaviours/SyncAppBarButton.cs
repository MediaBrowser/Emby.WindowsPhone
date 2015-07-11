using System.Windows;
using Cimbalino.Toolkit.Behaviors;
using MediaBrowser.Model.Dto;
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

        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(
            "Item", typeof (BaseItemDto), typeof (SyncAppBarButton), new PropertyMetadata(default(BaseItemDto), OnChanged));

        public BaseItemDto Item
        {
            get { return (BaseItemDto) GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public SyncAppBarButton()
        {
            SetIsVisible();
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
            if (Item == null || !SyncPolicy)
            {
                isVisible = false;
            }
            else
            {
                if (IsRemove)
                {
                    isVisible = Item.SyncStatus.HasValue && Item.SyncStatus.Value == SyncJobItemStatus.Synced;
                }
                else
                {
                    isVisible = !Item.SyncStatus.HasValue || Item.SyncStatus.Value != SyncJobItemStatus.Synced;
                }
            }

            IsVisible = isVisible;
        }
    }
}
