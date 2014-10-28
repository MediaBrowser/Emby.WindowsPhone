using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.ApiClient;
using Newtonsoft.Json;

namespace MediaBrowser.WindowsPhone.Model.Connection
{
    public class ServerLocator : IServerLocator
    {
        public async Task<List<ServerDiscoveryInfo>> FindServers(int timeoutMs, CancellationToken cancellationToken)
        {
            var result = new List<ServerDiscoveryInfo>();

            return result;
            var tcs = new TaskCompletionSource<List<ServerDiscoveryInfo>>();

            var ct = new CancellationTokenSource(timeoutMs);
            ct.Token.Register(() => tcs.SetResult(result), useSynchronizationContext: false);

            var socket = new DatagramSocket();
            socket.MessageReceived += (sender, args) =>
            {
                try
                {
                    var reader = args.GetDataReader();

                    uint stringLength = args.GetDataReader().UnconsumedBufferLength;
                    var rawData = args.GetDataReader().ReadString(stringLength);

                    var server = JsonConvert.DeserializeObject<ServerDiscoveryInfo>(rawData);

                    result.Add(server);
                }
                catch
                {
                    tcs.SetResult(result);
                }
            };

            try
            {
                using (var stream = await socket.GetOutputStreamAsync(new HostName("255.255.255.255"), "7359"))
                {
                    using (var writer = new DataWriter(stream))
                    {
                        var data = Encoding.UTF8.GetBytes("who is MediaBrowserServer_v2?");

                        writer.WriteBytes(data);
                        writer.StoreAsync();
                    }
                }
            }
            catch
            {
                tcs.SetResult(result);
            }

            return await tcs.Task;
        }
    }
}