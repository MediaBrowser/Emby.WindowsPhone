namespace MediaBrowser.WindowsPhone.Views.Settings
{
    public partial class MbConnectView 
    {
        // Constructor
        public MbConnectView()
        {
            InitializeComponent();
        }

        private void EmailLogs_OnClick(object sender, System.EventArgs e)
        {
            EmailLogs();
        }

        private void AboutItem_OnClick(object sender, System.EventArgs e)
        {
            AboutItem();
        }
    }
}