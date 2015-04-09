using System.Windows.Navigation;

namespace Emby.WindowsPhone.Views.Remote
{
    /// <summary>
    /// Description for ChooseClientView.
    /// </summary>
    public partial class ChooseClientView
    {
        /// <summary>
        /// Initializes a new instance of the ChooseClientView class.
        /// </summary>
        public ChooseClientView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }
    }
}