using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Windows8.Common;
using MediaBrowser.Windows8.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MediaBrowser.Model.Dto;
using MediaBrowser.Shared;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TvSeriesView : LayoutAwarePage
    {
        private BaseItemDto item;
        public TvSeriesView()
        {
            this.InitializeComponent();
            Loaded += async (sender, args) =>
                          {
                              var vm = ViewModelLocator.GetTvViewModel(item.Id);
                              var copy = new BaseItemDto();
                              Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                              {
                                  await Utils.CopyItem(item, copy);
                                  vm.SelectedTvSeries = copy;
                                  DataContext = vm;
                                  Messenger.Default.Send(new NotificationMessage(item.Id, Constants.Messages.TvShowPageLoadedMsg));
                              });
                          };
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<TvViewModel>(GetType());
            }
            else if(e.NavigationMode == NavigationMode.New)
            {
                item = (BaseItemDto) e.Parameter;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if(e.NavigationMode == NavigationMode.New)
            {
                History.Current.AddHistoryItem(GetType(), DataContext);                
            }
        }

        private void ItemGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<MainViewModel>().ItemClicked.Execute(e);
        }
    }
}
