using MediaBrowser.Model.Users;

namespace MediaBrowser.Model
{
    public class SettingsService : ISettingsService
    {
        public User LoggedInUser{ get; set; }
        public string HostName { get; set; }
        public string PortNo { get; set; }

        public string ApiUrl
        {
            get { return string.Format("http://{0}:{1}/mediabrowser/api/", HostName, PortNo); }
        }

        public bool CheckHostAndPort()
        {
            return !string.IsNullOrEmpty(HostName) && !string.IsNullOrEmpty(PortNo);
        }
    }
}
