using System;
using MediaBrowser.Model.ApiClient;

namespace Emby.WindowsPhone.Interfaces
{
    public interface IServerInfoService
    {
        bool HasServer { get; }
        ServerInfo ServerInfo { get; }
        void SetServerInfo(ServerInfo serverInfo);
        event EventHandler<ServerInfo> ServerInfoChanged;
    }
}
