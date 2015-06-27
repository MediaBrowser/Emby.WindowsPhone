using System.Windows.Navigation;

namespace Emby.WindowsPhone.Views.Settings
{
    public partial class FindServerView
    {
        // Constructor
        public FindServerView()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationService.CanGoBack && e.NavigationMode != NavigationMode.Back)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}