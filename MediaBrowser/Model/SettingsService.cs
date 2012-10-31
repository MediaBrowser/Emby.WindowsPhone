using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.DTO;
using System.ComponentModel;

namespace MediaBrowser.Model
{
    public class SettingsService : ISettingsService, INotifyPropertyChanged
    {
        public DtoUser LoggedInUser { get; set; }
        public string PinCode { get; set; }
        public ConnectionDetails ConnectionDetails { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }

        public string ApiUrl
        {
            get { return string.Format("http://{0}:{1}/mediabrowser/api/", ConnectionDetails.HostName, ConnectionDetails.PortNo); }
        }

        public bool CheckHostAndPort()
        {
            return !string.IsNullOrEmpty(ConnectionDetails.HostName);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
