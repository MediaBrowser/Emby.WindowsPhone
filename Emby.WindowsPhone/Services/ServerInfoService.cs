using System;
using MediaBrowser.Model.ApiClient;
using Emby.WindowsPhone.Interfaces;

namespace Emby.WindowsPhone.Services
{
    public class ServerInfoService : IServerInfoService
    {
        public bool HasServer
        {
            get { return ServerInfo != null && !string.IsNullOrEmpty(ServerInfo.Id); }
        }

        public ServerInfo ServerInfo { get; private set; }

        public void SetServerInfo(ServerInfo serverInfo)
        {
            ServerInfo = serverInfo;

            SendEvent();
        }

        private void SendEvent()
        {
            var eventHandler = ServerInfoChanged;
            if (eventHandler != null)
            {
                eventHandler(this, ServerInfo);
            }
        }

        public event EventHandler<ServerInfo> ServerInfoChanged;
    }
}
