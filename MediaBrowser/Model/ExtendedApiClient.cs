using System.Threading.Tasks;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Web;

namespace MediaBrowser.WindowsPhone.Model
{
    public class ExtendedApiClient : ApiClient
    {
        public ExtendedApiClient(IAsyncHttpClient httpClient)
            : base(httpClient)
        {
            
        }

        public bool IsBusy
        {
            get
            {
                var asyncHttpClient = HttpClient as AsyncHttpClient;
                return asyncHttpClient != null && asyncHttpClient.IsBusy;
            }
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

            return await GetStream(url);
        }

        public async Task<RequestResult> CheckForPushServer()
        {
            var url = GetApiUrl("push");

            return await GetStream(url);
        }

        public async Task<RequestResult> CheckForPulse(string deviceId)
        {
            var dict = new QueryStringDictionary {{"deviceid", deviceId}, {"action", "heartbeat"}};

            var url = GetApiUrl("push", dict);

            return await GetStream(url);
        }

        public async Task<RequestResult> UpdateDevice(string deviceId, bool? sendToasts = null, bool? liveTile = null)
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

            var url = GetApiUrl("push", dict);

            return await GetStream(url);
        }

        public async Task<RequestResult> DeleteDevice(string deviceId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, {"action", "delete"} };

            var url = GetApiUrl("push", dict);

            return await GetStream(url);
        }

        public async Task<RequestResult> GetDeviceSettings(string deviceId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, { "action", "getsettings" } };

            var url = GetApiUrl("push", dict);

            return await GetStream(url);
        }

        private async Task<RequestResult> GetStream(string url)
        {
            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<RequestResult>(stream);
            }
        }
    }
}
