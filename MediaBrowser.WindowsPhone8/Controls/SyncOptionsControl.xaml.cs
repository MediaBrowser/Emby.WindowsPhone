using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Model.Sync;
using ScottIsAFool.WindowsPhone.Logging;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace MediaBrowser.WindowsPhone.Controls
{
    public partial class SyncOptionsControl
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILog _logger;

        public static readonly DependencyProperty SyncUnwatchedProperty = DependencyProperty.Register(
            "SyncUnwatched", typeof (bool), typeof (SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncUnwatched
        {
            get { return (bool) GetValue(SyncUnwatchedProperty); }
            set { SetValue(SyncUnwatchedProperty, value); }
        }

        public static readonly DependencyProperty SyncNewContentProperty = DependencyProperty.Register(
            "SyncNewContent", typeof (bool), typeof (SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncNewContent
        {
            get { return (bool) GetValue(SyncNewContentProperty); }
            set { SetValue(SyncNewContentProperty, value); }
        }

        public static readonly DependencyProperty ItemLimitProperty = DependencyProperty.Register(
            "ItemLimit", typeof (string), typeof (SyncOptionsControl), new PropertyMetadata(default(string)));

        public string ItemLimit
        {
            get { return (string) GetValue(ItemLimitProperty); }
            set { SetValue(ItemLimitProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", typeof (bool), typeof (SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public SyncOptionsControl()
        {
            InitializeComponent();
            DataContext = this;
            _connectionManager = SimpleIoc.Default.GetInstance<IConnectionManager>();
            _logger = new WPLogger(GetType());
        }

        private void SyncButton_OnTap(object sender, GestureEventArgs e)
        {
            Close();
        }

        private void Close()
        {
            if (MessagePrompt != null)
            {
                MessagePrompt.OnCompleted(new PopUpEventArgs<string, PopUpResult> {PopUpResult = PopUpResult.Ok});
            }
        }

        public MessagePrompt MessagePrompt { get; set; }

        private SyncDialogOptions _options;
        private bool _unwatched, _itemLimit, _autoSync;
        public async Task SetOptions(SyncJobRequest request)
        {
            IsLoading = true;
            var apiClient = _connectionManager.GetApiClient(App.ServerInfo.Id);

            try
            {
                _logger.Info("Getting sync options");
                _options = await apiClient.GetSyncOptions(request);
                if (_options != null)
                {
                    _unwatched = _options.Options.Contains(SyncJobOption.UnwatchedOnly);
                    _itemLimit = _options.Options.Contains(SyncJobOption.ItemLimit);
                    _autoSync = _options.Options.Contains(SyncJobOption.SyncNewContent);

                    OptionsPicker.ItemsSource = _options.QualityOptions;
                    OptionsPicker.SelectedItem = OptionsPicker.Items.FirstOrDefault(x => (x as SyncQualityOption).IsDefault);
                    ItemLimitBox.Visibility = _itemLimit ? Visibility.Visible : Visibility.Collapsed;
                    AutoSyncVideos.Visibility = _autoSync ? Visibility.Visible : Visibility.Collapsed;
                    UnwatchedVideos.Visibility = _unwatched ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    Close();
                }
            }
            catch (HttpException ex)
            {
                Close();
                Utils.HandleHttpException("SetOptions()", ex, SimpleIoc.Default.GetInstance<INavigationService>(), _logger);
            }

            IsLoading = false;
        }

        public SyncOption GetSelectedOption()
        {
            var item = OptionsPicker.SelectedItem as SyncQualityOption;
            
            var option = new SyncOption
            {
                Quality = item,
                AutoSyncNewItems = _autoSync && SyncNewContent,
                UnwatchedItems = _unwatched && SyncUnwatched
            };

            if (_itemLimit && !string.IsNullOrEmpty(ItemLimit))
            {
                option.ItemLimit = int.Parse(ItemLimit);
            }

            return option;
        }
    }
}
