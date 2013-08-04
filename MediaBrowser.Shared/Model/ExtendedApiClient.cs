using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Web;

namespace MediaBrowser.Model
{
    public class ExtendedApiClient : ApiInteraction.ApiClient, IExtendedApiClient
    {
        public ExtendedApiClient(ILogger logger, string serverHostName, int serverApiPort, string clientName, string deviceName, string deviceId, string appVersion)
            : base(logger, serverHostName, serverApiPort, clientName, deviceName, deviceId, appVersion)
        {
            
        }

        /// <summary>
        /// Registers the device for push notifications.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="sendTileUpdate">The send tile update.</param>
        /// <param name="sendToastUpdate">The send toast update.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceType
        /// or
        /// deviceId</exception>
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
#if WP8
                               {"devicetype", "WindowsPhone8"}
#else
                               {"devicetype", "WindowsPhone7"}
#endif
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
        /// <exception cref="ArgumentNullException">deviceId</exception>
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
        /// <exception cref="ArgumentNullException">deviceId</exception>
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
        /// <exception cref="ArgumentNullException">deviceId</exception>
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
        /// <exception cref="ArgumentNullException">deviceId</exception>
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

        private async Task<T> GetStream<T>(string url) where T : class
        {
            using (var stream = await GetSerializedStreamAsync(url).ConfigureAwait(false))
            {
                //return DeserializeFromStream<T>(stream);
                return (T)JsonSerializer.DeserializeFromStream(stream, typeof (T));
            }
        }
    }
}
