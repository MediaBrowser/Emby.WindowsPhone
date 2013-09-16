using System;
using Cimbalino.Phone.Toolkit.Services;
using Microsoft.Phone.Marketplace;

namespace MediaBrowser.WindowsPhone.Services
{
    public class TrialService
    {
        private static TrialService _current;
        private readonly ApplicationSettingsService _settings;

        private const string VideoItem = "VideoItem";
        
        public TrialService()
        {
            _settings = new ApplicationSettingsService();
            IsTrial = new LicenseInformation().IsTrial();
        }

        public static TrialService Current
        {
            get { return _current ?? (_current = new TrialService()); }
        }

        public bool IsTrial { get; private set; }

        public void Buy()
        {
            new Microsoft.Phone.Tasks.MarketplaceDetailTask().Show();
        }

        public bool CheckVideoId(string id)
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

            if (DateTime.Now.Date != item.Date.Date)
            {
                return true;
            }

            if (item.Id.Equals(id))
            {
                return true;
            }

            return false;
        }

        public void SetNewVideoItem(string id)
        {
            var item = new TrialVideoItem
            {
                Date = DateTime.Now,
                Id = id
            };

            _settings.Set(VideoItem, item);
            _settings.Save();
        }

        public bool CheckDeviceType(string deviceType)
        {
            if (!IsTrial)
            {
                return true;
            }

            return deviceType.ToLower().Equals("dashboard");
        } 

        public class TrialVideoItem
        {
            public string Id { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
