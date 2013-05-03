using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
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

            Loaded += (s, e) =>
            {
                var item = (DataContext as FolderViewModel).SelectedFolder;
                var url = (string)
                          new Converters.ImageUrlConverter().
                              Convert(item, typeof(string), "backdrop", null);
                if (!string.IsNullOrEmpty(url))
                {
                    ThePanorama.Background = new ImageBrush
                    {
                        Stretch = Stretch.UniformToFill,
                        Opacity = 0.2,
                        ImageSource = new BitmapImage(new Uri(url))
                    };
                }
            };
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