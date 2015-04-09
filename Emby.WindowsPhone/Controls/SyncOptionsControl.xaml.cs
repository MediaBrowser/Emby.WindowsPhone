using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using Emby.WindowsPhone.Extensions;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Model.Sync;
using Emby.WindowsPhone.Localisation;
using ScottIsAFool.WindowsPhone.Logging;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace Emby.WindowsPhone.Controls
{
    public partial class SyncOptionsControl
    {
        private readonly IConnectionManager _connectionManager;
        private readonly ILog _logger;

        public static readonly DependencyProperty SyncUnwatchedProperty = DependencyProperty.Register(
            "SyncUnwatched", typeof(bool), typeof(SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncUnwatched
        {
            get { return (bool)GetValue(SyncUnwatchedProperty); }
            set { SetValue(SyncUnwatchedProperty, value); }
        }

        public static readonly DependencyProperty SyncNewContentProperty = DependencyProperty.Register(
            "SyncNewContent", typeof(bool), typeof(SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool SyncNewContent
        {
            get { return (bool)GetValue(SyncNewContentProperty); }
            set { SetValue(SyncNewContentProperty, value); }
        }

        public static readonly DependencyProperty ItemLimitProperty = DependencyProperty.Register(
            "ItemLimit", typeof(string), typeof(SyncOptionsControl), new PropertyMetadata(default(string)));

        public string ItemLimit
        {
            get { return (string)GetValue(ItemLimitProperty); }
            set { SetValue(ItemLimitProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
            "IsLoading", typeof(bool), typeof(SyncOptionsControl), new PropertyMetadata(default(bool)));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
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
                MessagePrompt.OnCompleted(new PopUpEventArgs<string, PopUpResult> { PopUpResult = PopUpResult.Ok });
            }
        }

        public MessagePrompt MessagePrompt { get; set; }

        private SyncDialogOptions _options;
        private bool _unwatched, _itemLimit, _autoSync, _profileNeeded, _qualityNeeded;
        private SyncJobRequest _request;
        public async Task SetOptions(SyncJobRequest request)
        {
            IsLoading = true;
            _request = request;
            var apiClient = _connectionManager.GetApiClient(App.ServerInfo.Id);

            try
            {
                _logger.Info("Getting sync options");
                _options = await apiClient.GetSyncOptions(request);
                if (_options != null && _options.Targets.Any())
                {
                    var thisDevice = _options.Targets.FirstOrDefault(x => x.Id == apiClient.DeviceId);
                    if (thisDevice == null)
                    {
                        TargetDevices.ItemsSource = _options.Targets;
                        TargetDevices.SelectedItem = _options.Targets.FirstOrDefault();
                    }
                    else
                    {
                        thisDevice.Name = string.Format(AppResources.LabelThisDevice, thisDevice.Name);
                        _options.Targets.Remove(thisDevice);
                        _options.Targets.Insert(0, thisDevice);

                        TargetDevices.ItemsSource = _options.Targets;
                        TargetDevices.SelectedItem = thisDevice;
                        //GetOptionsForDevice(thisDevice.Id).ConfigureAwait(false);
                    }
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

        private async Task GetOptionsForDevice(string deviceId)
        {
            IsLoading = true;
            _request.TargetId = deviceId;
            var apiClient = _connectionManager.GetApiClient(App.ServerInfo.Id);

            try
            {
                _logger.Info("Getting sync options for device ({0})", deviceId);
                _options = await apiClient.GetSyncOptions(_request);
                if (_options != null)
                {
                    _unwatched = _options.Options.Contains(SyncJobOption.UnwatchedOnly);
                    _itemLimit = _options.Options.Contains(SyncJobOption.ItemLimit);
                    _autoSync = _options.Options.Contains(SyncJobOption.SyncNewContent);
                    _profileNeeded = _options.Options.Contains(SyncJobOption.Profile);
                    _qualityNeeded = _options.Options.Contains(SyncJobOption.Quality);

                    if (_qualityNeeded)
                    {
                        QualityPicker.ItemsSource = _options.QualityOptions;
                        QualityPicker.SelectedItem = QualityPicker.Items.FirstOrDefault(x => (x as SyncQualityOption).IsDefault);
                    }

                    if (_profileNeeded)
                    {
                        var profiles = _options.ProfileOptions.Select(x => new SyncProfileOption {Name = x.GetName(), Description = x.GetDescription(), Id = x.Id, IsDefault = x.IsDefault}).ToList();
                        ProfilePicker.ItemsSource = profiles;
                        ProfilePicker.SelectedItem = ProfilePicker.Items.FirstOrDefault(x => (x as SyncProfileOption).IsDefault);
                    }

                    ProfilePicker.Visibility = _profileNeeded ? Visibility.Visible : Visibility.Collapsed;
                    QualityPicker.Visibility = _qualityNeeded ? Visibility.Visible : Visibility.Collapsed;
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
            var item = QualityPicker.SelectedItem as SyncQualityOption;
            
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

            if (_profileNeeded)
            {
                var profile = ProfilePicker.SelectedItem as SyncProfileOption;
                option.Profile = profile;
            }

            return option;
        }

        private void TargetDevices_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var device = e.AddedItems[0] as SyncTarget;
                if (device != null)
                {
                    GetOptionsForDevice(device.Id).ConfigureAwait(false);
                }
            }
        }

        private void ProfilePicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var profileOption = ProfilePicker.SelectedItem as SyncProfileOption;
            if (profileOption != null)
            {
                QualityPicker.Visibility = profileOption.EnableQualityOptions ? Visibility.Visible : Visibility.Collapsed;

                ProfileDescription.Text = profileOption.Description;
                ProfileDescription.Visibility = string.IsNullOrEmpty(profileOption.Description) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
