using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.DTO;

namespace MediaBrowser.Model
{
    public class SettingsService : ISettingsService
    {
        public DtoUser LoggedInUser{ get; set; }
        public string HostName { get; set; }
        public int PortNo { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }

        public string ApiUrl
        {
            get { return string.Format("http://{0}:{1}/mediabrowser/api/", HostName, PortNo); }
        }

        public bool CheckHostAndPort()
        {
            return !string.IsNullOrEmpty(HostName);
        }
    }
}
