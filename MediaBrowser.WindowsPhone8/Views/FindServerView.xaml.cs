using System.Windows.Navigation;

namespace MediaBrowser.WindowsPhone.Views
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

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationService.CanGoBack && e.NavigationMode != NavigationMode.Back)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}