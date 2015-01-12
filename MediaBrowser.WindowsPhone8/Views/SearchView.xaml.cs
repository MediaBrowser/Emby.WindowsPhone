using System.Windows.Input;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for SearchView.
    /// </summary>
    public partial class SearchView
    {
        /// <summary>
        /// Initializes a new instance of the SearchView class.
        /// </summary>
        public SearchView()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }
    }
}