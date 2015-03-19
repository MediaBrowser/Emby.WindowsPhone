using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Interfaces;

namespace MediaBrowser.WindowsPhone.Services
{
    public class ServerInfoService : IServerInfoService
    {
        public ServerInfo ServerInfo { get; private set; }

        public void SetServerInfo(ServerInfo serverInfo)
        {
            ServerInfo = serverInfo;
        }
    }
}
