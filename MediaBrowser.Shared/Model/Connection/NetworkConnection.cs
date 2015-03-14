using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.ApiClient;
using Microsoft.Phone.Net.NetworkInformation;

namespace MediaBrowser.WindowsPhone.Model.Connection
{
    public class NetworkConnection : INetworkConnection
    {
        public async Task SendWakeOnLan(string macAddress, string ipAddress, int port, CancellationToken cancellationToken)
        {
            await DoWakeOnLan(macAddress, ipAddress, port.ToString(CultureInfo.InvariantCulture));
        }
        
        public async Task SendWakeOnLan(string macAddress, int port, CancellationToken cancellationToken)
        {
            await DoWakeOnLan(macAddress, "255.255.255.255", "7");
        }

        public NetworkStatus GetNetworkStatus()
        {
            var result = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            return new NetworkStatus
            {
                IsNetworkAvailable = result && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None
            };
        }

        public event EventHandler<EventArgs> NetworkChanged;

        private async Task DoWakeOnLan(string macAddress, string ipAddress, string port)
        {
            var socket = new DatagramSocket();

            await socket.BindServiceNameAsync("0");

            var o = await socket.GetOutputStreamAsync(new HostName(ipAddress), port);
            var writer = new DataWriter(o);
            writer.WriteBytes(GetBuffer(macAddress));
            await writer.StoreAsync();

            socket.Dispose();
        }

        private Byte[] GetBuffer(string macAddress)
        {
            var buf = new char[102];
            Byte[] sendBytes = Encoding.UTF8.GetBytes(buf);

            for (int x = 0; x < 6; x++)
            {
                sendBytes[x] = 0xff;
            }

            string[] macDigits = null;
            if (macAddress.Contains("-"))
            {
                macDigits = macAddress.Split('-');
            }
            else
            {
                macDigits = macAddress.Split(':');
            }

            int start = 6;
            for (int i = 0; i < 16; i++)
            {
                for (int x = 0; x < 6; x++)
                {
                    sendBytes[start + i * 6 + x] = (byte)Convert.ToInt32(macDigits[x], 16);
                }
            }

            return sendBytes;
        }
    }
}