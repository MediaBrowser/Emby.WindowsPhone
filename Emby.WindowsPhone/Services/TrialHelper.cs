using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone.Logging;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.Services
{
    public class TrialHelper : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private const string VideoItem = "VideoItem";
        private readonly IApplicationSettingsServiceHandler _settings;
        private readonly ILog _logger;

        public TrialHelper(INavigationService navigationService, IApplicationSettingsService applicationSettings)
        {
            _navigationService = navigationService;
            _settings = applicationSettings.Legacy;
#if TRIAL
            IsTrial = true;
#else
            CheckLicences().ConfigureAwait(false);
#endif
            _logger = new WPLogger(GetType());

            Current = this;
        }

        private Task CheckLicences()
        {
            var freeLicence = CurrentApp.LicenseInformation.ProductLicenses[Constants.RemoveAdsProductFree];
            var paidLicence = CurrentApp.LicenseInformation.ProductLicenses[Constants.RemoveAdsProduct];
            IsTrial = !(freeLicence.IsActive || paidLicence.IsActive);
            //_settings.Set(Constants.Settings.AppIsBought, !IsTrial);

            return Task.FromResult(0);
        }

        public static TrialHelper Current { get; private set; }

        public bool IsTrial { get; private set; }

        public async Task<bool> Buy()
        {
            try
            {
                var isBought = _settings.Get(Constants.Settings.AppIsBought, false);
                var licenceId = isBought ? Constants.RemoveAdsProductFree : Constants.RemoveAdsProduct;

                await CurrentApp.RequestProductPurchaseAsync(licenceId, false);

                IsTrial = false;
                _settings.Set(Constants.Settings.AppIsBought, !IsTrial);

                return true;
            }
            catch
            {
                _logger.Info("User most likely cancelled purchased");
            }

            return false;
        }

        public bool CanPlayVideo(string id)
        {
            if (!IsTrial)
            {
                return true;
            }

            var item = _settings.Get<TrialVideoItem>(VideoItem, null);
            if (item == null)
            {
                return true;
            }

            return DateTime.Now.Date != item.Date.Date || item.Id.Equals(id);
        }

        public void SetNewVideoItem(string id)
        {
            var item = new TrialVideoItem
            {
                Date = DateTime.Now,
                Id = id
            };

            _settings.Set(VideoItem, item);
        }

        public void ShowTrialMessage(string message)
        {
            var messageBox = new CustomMessageBox
            {
                Title = "Trial",
                Message = message,
                LeftButtonContent = "purchase",
                RightButtonContent = "ok"
            };

            messageBox.Dismissed += (sender, e) =>
            {
                if (e.Result == CustomMessageBoxResult.LeftButton)
                {
                    _navigationService.Navigate("");
                }
            };

            messageBox.Show();
        }

        public bool CanRemoteControl(string deviceType)
        {
            return !IsTrial || deviceType.ToLower().Equals("dashboard");
        }

        public class TrialVideoItem
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
        }
    }
}