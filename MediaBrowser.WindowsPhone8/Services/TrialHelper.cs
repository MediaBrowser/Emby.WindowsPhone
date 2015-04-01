using Cimbalino.Toolkit.Services;
#if !TRIAL
using Microsoft.Phone.Marketplace;
#endif
using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace MediaBrowser.WindowsPhone.Services
{
    public class TrialHelper
    {
        private const string VideoItem = "VideoItem";
        private static TrialHelper _current;
        private readonly IApplicationSettingsServiceHandler _settings;

        public TrialHelper()
        {
            _settings = new ApplicationSettingsService().Legacy;
#if TRIAL
            IsTrial = true;
#else
            IsTrial = new LicenseInformation().IsTrial();
            _settings.Set(Constants.Settings.AppIsBought, !IsTrial);
#endif
        }

        public static TrialHelper Current
        {
            get { return _current ?? (_current = new TrialHelper()); }
        }

        public bool IsTrial { get; private set; }

        public void Buy()
        {
            new MarketplaceDetailTask().Show();
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
                    Buy();
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