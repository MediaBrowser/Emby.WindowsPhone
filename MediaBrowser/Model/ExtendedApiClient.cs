using System.Threading.Tasks;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Web;

namespace MediaBrowser.WindowsPhone.Model
{
    public class ExtendedApiClient : ApiClient
    {
        public ExtendedApiClient(ILogger logger, IAsyncHttpClient httpClient)
            : base(logger, httpClient)
        {
            
        }

        public async Task<RequestResult> RegisterDevice(string deviceId, string uri, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            var dict = new QueryStringDictionary {{"deviceid", deviceId}, {"url", uri}, {"action", "register"}, 
#if WP8
            {"devicetype", "WindowsPhone8"}
#else
            {"devicetype", "WindowsPhone7"}
#endif
            };

            if(sendTileUpdate.HasValue)
                dict.Add("sendlivetile", sendTileUpdate.Value);
            if(sendToastUpdate.HasValue)
                dict.Add("sendtoast", sendToastUpdate.Value);

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<RequestResult> CheckForPushServer()
        {
            var url = GetApiUrl("push");

            return await GetStream<RequestResult>(url);
        }

        public async Task<RequestResult> CheckForPulse(string deviceId)
        {
            var dict = new QueryStringDictionary {{"deviceid", deviceId}, {"action", "heartbeat"}};

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<RequestResult> UpdateDevice(string deviceId, bool? sendToasts = null, bool? liveTile = null, string liveTileName = null, string liveTileId = null)
        {
            var dict = new QueryStringDictionary {{"deviceid", deviceId}, {"action", "update"}};

            if (sendToasts.HasValue)
            {
                dict.Add("sendtoast", sendToasts.Value);
            }

            if (liveTile.HasValue)
            {
                dict.Add("sendlivetile", liveTile.Value);
            }

            if (!string.IsNullOrEmpty(liveTileName) && !string.IsNullOrEmpty(liveTileId))
            {
                dict.Add("tilename", liveTileName);
                dict.Add("tileid", liveTileId);
            }

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<RequestResult> DeleteDevice(string deviceId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, {"action", "delete"} };

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<RequestResult> DeleteLiveTile(string deviceId, string liveTileId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, { "action", "deletetile" }, {"tileid", liveTileId} };

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<DeviceSettings> GetDeviceSettings(string deviceId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, { "action", "getsettings" } };

            var url = GetApiUrl("push", dict);

            return await GetStream<DeviceSettings>(url, SerializationFormats.Json);
        }

        private async Task<T> GetStream<T>(string url, SerializationFormats format = SerializationFormats.Protobuf) where T : class
        {
            using (var stream = await GetSerializedStreamAsync(url, format).ConfigureAwait(false))
            {
                //return DeserializeFromStream<T>(stream);
                return (T)DataSerializer.DeserializeFromStream(stream, format, typeof (T));
            }
        }
    }
}
