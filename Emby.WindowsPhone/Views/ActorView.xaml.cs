using System.Windows.Navigation;
using MediaBrowser;
using MediaBrowser.Model.Dto;
using Emby.WindowsPhone.ViewModel;

namespace Emby.WindowsPhone.Views
{
    public partial class ActorView
    {
        public ActorView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<ActorViewModel>(GetType());
                App.SelectedItem = ((ActorViewModel) DataContext).SelectedPerson;
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                if (App.SelectedItem is BaseItemPerson)
                {
                    DataContext = new ActorViewModel(ViewModelLocator.ConnectionManager, ViewModelLocator.NavigationService)
                    {
                        SelectedPerson = (BaseItemPerson) App.SelectedItem
                    };
                }
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