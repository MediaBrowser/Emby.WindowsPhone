using System;
using System.Windows.Navigation;
using Emby.WindowsPhone.Services;

namespace Emby.WindowsPhone.Views.Settings
{
    public partial class TileSettingsView
    {
        // Constructor
        public TileSettingsView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                TileService.Current.SetSecondaryTileTransparency(App.SpecificSettings.UseTransparentTile);
                TileService.Current.UpdatePrimaryTile(App.SpecificSettings.DisplayBackdropOnTile, App.SpecificSettings.UseRichWideTile, App.SpecificSettings.UseTransparentTile).ConfigureAwait(false);
            }

            base.OnNavigatedFrom(e);
        }

        public void EmailLogs_OnClick(object sender, EventArgs e)
        {
            EmailLogs();
        }

        public void AboutItem_OnClick(object sender, EventArgs e)
        {
            AboutItem();
        }
    }
}