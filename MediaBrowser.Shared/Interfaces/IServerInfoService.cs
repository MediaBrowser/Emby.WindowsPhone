using MediaBrowser.Model.ApiClient;

namespace MediaBrowser.WindowsPhone.Interfaces
{
    public interface IServerInfoService
    {
        ServerInfo ServerInfo { get; }
        void SetServerInfo(ServerInfo serverInfo);
    }
}
