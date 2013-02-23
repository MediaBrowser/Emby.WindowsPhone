using Callisto.Controls.SettingsManagement;

namespace MediaBrowser.Windows8
{
    public static class SettingsCharm
    {
        public static void Create()
        {
            AppSettings.Current.AddCommand<SettingsViews.GeneralSettings>("General");
            AppSettings.Current.AddCommand<SettingsViews.ConnectionSettings>("Connection");
            AppSettings.Current.AddCommand<SettingsViews.ImageSettings>("Images");
            AppSettings.Current.AddCommand<SettingsViews.PushNotificationsSettings>("Push Notifications");
            //AppSettings.Current.AddCommand<SettingsViews.StreamingSettings>("Streaming");
        }
    }
}
