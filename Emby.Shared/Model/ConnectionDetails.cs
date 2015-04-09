using PropertyChanged;

namespace Emby.WindowsPhone.Model
{
    [ImplementPropertyChanged]
    public class ConnectionDetails
    {
        public string HostName { get; set; }
        public int PortNo { get; set; }
        public string ServerId { get; set; }

        public string DisplayUrl
        {
            get { return string.Format("http://{0}:{1}/mediabrowser", HostName, PortNo); }
        }

        public string ServerAddress
        {
            get
            {
                return string.Format("http://{0}:{1}", HostName, PortNo);
            }
        }
    }
}