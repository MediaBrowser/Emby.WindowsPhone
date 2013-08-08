using System;
using System.Threading.Tasks;
using MediaBrowser.Model.ApiClient;

namespace MediaBrowser.Model
{
    public interface IExtendedApiClient : IApiClient
    {
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
        Task RegisterDeviceAsync(string deviceId, string uri, bool? sendTileUpdate = null, bool? sendToastUpdate = null);

        /// <summary>
        /// Deletes the device async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceId</exception>
        Task DeleteDeviceAsync(string deviceId);

        /// <summary>
        /// Updates the device async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <param name="sendTileUpdate">The send tile update.</param>
        /// <param name="sendToastUpdate">The send toast update.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceId</exception>
        Task UpdateDeviceAsync(string deviceId, bool? sendTileUpdate = null, bool? sendToastUpdate = null);

        /// <summary>
        /// Pushes the heartbeat async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceId</exception>
        Task PushHeartbeatAsync(string deviceId);

        /// <summary>
        /// Gets the device settings async.
        /// </summary>
        /// <param name="deviceId">The device id.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">deviceId</exception>
        Task<DeviceSettings> GetDeviceSettingsAsync(string deviceId);
    }
}