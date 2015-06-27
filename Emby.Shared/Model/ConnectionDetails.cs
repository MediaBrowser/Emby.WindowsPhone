using PropertyChanged;

namespace Emby.WindowsPhone.Model
{
    [ImplementPropertyChanged]
    public class ConnectionDetails
    {
        public string HostName { get; set; }
        public int PortNo { get; set; }
        public string ServerId { get; set; }
        public bool IsHttps { get; set; }

        public string DisplayUrl
        {
            get
            {
                var protocol = IsHttps ? "https" : "http";
                return string.Format("{0}://{1}:{2}/emby", protocol, HostName, PortNo);
            }
        }

        public string ServerAddress
        {
            get
            {
                var protocol = IsHttps ? "https" : "http";
                return string.Format("{0}://{1}:{2}", protocol, HostName, PortNo);
            }
        }
    }
}