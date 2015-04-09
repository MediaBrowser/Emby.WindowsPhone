using System.Windows.Navigation;
using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.ViewModel;
using Emby.WindowsPhone.ViewModel.Channels;

namespace Emby.WindowsPhone.Views.Channels
{
    public partial class ChannelView
    {
        // Constructor
        public ChannelView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<ChannelViewModel>(GetType());
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                var item = (BaseItemDto)App.SelectedItem;
                var vm = ViewModelLocator.GetChannelViewModel(item.Id);
                vm.SelectedChannel = item;
                DataContext = vm;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);
            }
        }
    }
}