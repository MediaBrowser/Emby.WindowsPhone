using System.Windows.Controls;
using System.Windows.Navigation;
using MediaBrowser.Model.DTO;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for CollectionView.
    /// </summary>
    public partial class CollectionView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the CollectionView class.
        /// </summary>
        public CollectionView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                var selectedItem = new DtoBaseItem();
                if(App.SelectedItem == null)
                {
                    string name, id;
                    if (NavigationContext.QueryString.TryGetValue("name", out name) &&
                        NavigationContext.QueryString.TryGetValue("id", out id))
                    {
                        selectedItem = new DtoBaseItem
                                           {
                                               Name = name,
                                               Id = id,
                                               Type = "FolderCollection"
                                           };
                    }
                }
                if (App.SelectedItem is DtoBaseItem)
                {
                    selectedItem = (DtoBaseItem) App.SelectedItem;
                }
                DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                {
                    SelectedFolder = selectedItem
                };
            }
        }

        private void Panorama_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ApplicationBar.IsVisible = ThePanorama.SelectedIndex == 0;
        }
    }
}