using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Web;
using MediaBrowser.Shared;

namespace MediaBrowser.Windows8.Model
{
    public class ExtendedApiClient : ApiClient
    {
        public ExtendedApiClient(ILogger logger, IAsyncHttpClient httpClient, string serverHostName, int serverApiPort, string clientName, string deviceName, string deviceId, string appVersion)
            : base(logger, httpClient, serverHostName, serverApiPort, clientName, deviceName, deviceId, appVersion)
        {
            
        }

        /// <summary>
        /// Registers the device for push notifications.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="deviceType">Type of the device.</param>
        /// <param name="sendTileUpdate">The send tile update.</param>
        /// <param name="sendToastUpdate">The send toast update.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// deviceType
        /// or
        /// deviceId
        /// </exception>
        public Task RegisterDeviceAsync(string deviceId, string uri, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var dict = new QueryStringDictionary
                           {
                               {"deviceid", deviceId},
                               {"url", uri},
                               {"devicetype", "Windows8"}
                           };

            if (sendTileUpdate.HasValue)
                dict.Add("sendlivetile", sendTileUpdate.Value);
            if (sendToastUpdate.HasValue)
                dict.Add("sendtoast", sendToastUpdate.Value);

            var url = GetApiUrl("PushNotification/Devices", dict);

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>());
        }

        /// <summary>
        /// Deletes the device async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">deviceId</exception>
        public Task DeleteDeviceAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var url = GetApiUrl("PushNotification/Devices/" + deviceId);

            return HttpClient.DeleteAsync(url, CancellationToken.None);
        }

        /// <summary>
        /// Updates the device async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="sendTileUpdate">The send tile update.</param>
        /// <param name="sendToastUpdate">The send toast update.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">deviceId</exception>
        public Task UpdateDeviceAsync(string deviceId, bool? sendTileUpdate = null, bool? sendToastUpdate = null)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var dict = new QueryStringDictionary();

            if (sendTileUpdate.HasValue)
                dict.Add("sendlivetile", sendTileUpdate.Value);
            if (sendToastUpdate.HasValue)
                dict.Add("sendtoast", sendToastUpdate.Value);

            var url = GetApiUrl("PushNotification/Devices/" + deviceId, dict);

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>());
        }

        /// <summary>
        /// Pushes the heartbeat async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">deviceId</exception>
        public Task PushHeartbeatAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var url = GetApiUrl("PushNotification/Devices/" + deviceId + "/Heartbeats");

            return PostAsync<EmptyRequestResult>(url, new Dictionary<string, string>());
        }

        /// <summary>
        /// Gets the device settings async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">deviceId</exception>
        public async Task<DeviceSettings> GetDeviceSettingsAsync(string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ArgumentNullException("deviceId");
            }

            var url = GetApiUrl("PushNotification/Devices/" + deviceId + "/Settings");

            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<DeviceSettings>(stream);
            }
        }

        public async Task<RequestResult> DeleteLiveTile(string deviceId, string liveTileId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, { "action", "deletetile" }, { "tileid", liveTileId } };

            var url = GetApiUrl("push", dict);

            return await GetStream<RequestResult>(url);
        }

        public async Task<object> GetDeviceSettings(string deviceId)
        {
            var dict = new QueryStringDictionary { { "deviceid", deviceId }, { "action", "getsettings" } };

            var url = GetApiUrl("push", dict);

            return await GetStream<object>(url);
        }

        private async Task<T> GetStream<T>(string url) where T : class
        {
            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                return DeserializeFromStream<T>(stream);
            }
        }
    }
}
