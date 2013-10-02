using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.Views
{
    /// <summary>
    /// Description for CollectionView.
    /// </summary>
    public partial class CollectionView
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
                var selectedItem = new BaseItemDto();
                if(App.SelectedItem == null)
                {
                    string name, id;
                    if (NavigationContext.QueryString.TryGetValue("name", out name) &&
                        NavigationContext.QueryString.TryGetValue("id", out id))
                    {
                        selectedItem = new BaseItemDto
                                           {
                                               Name = name,
                                               Id = id,
                                               Type = "FolderCollection"
                                           };
                    }
                }
                if (App.SelectedItem is BaseItemDto)
                {
                    selectedItem = (BaseItemDto) App.SelectedItem;
                }
                DataContext = new FolderViewModel(ViewModelLocator.NavigationService, ViewModelLocator.ApiClient)
                {
                    SelectedFolder = selectedItem
                };
            }
        }
    }
}