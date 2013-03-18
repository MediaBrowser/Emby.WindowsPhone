using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    public partial class FindServerView : PhoneApplicationPage
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
            if (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }
    }
}