using System;
using Ailon.WP.Utils;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.WindowsPhone.Model.Connection;
using MediaBrowser.WindowsPhone.Model.Security;
using Microsoft.Phone.Info;

namespace MediaBrowser.WindowsPhone
{
    public static class SharedUtils
    {
        public static string GetDeviceName()
        {
            var deviceName = DeviceStatus.DeviceName;
            var deviceId = DeviceStatus.DeviceManufacturer;
            var phone = PhoneNameResolver.Resolve(deviceId, deviceName);
            var deviceInfo = string.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

            return deviceInfo;
        }

        public static string GetDeviceId()
        {
            var uniqueId = new DeviceExtendedPropertiesService().DeviceUniqueId;
            var deviceId = Convert.ToBase64String(uniqueId, 0, uniqueId.Length);

            return deviceId;
        }

        public static IConnectionManager CreateConnectionManager(IDevice device, ILogger logger)
        {
            var manager = new ConnectionManager(
                logger,
                new CredentialProvider(),
                new NetworkConnection(),
                new ServerLocator(),
                "Windows Phone 8",
                ApplicationManifest.Current.App.Version,
                device,
                WindowsPhoneCapabilities.App,
                new CryptographyProvider(),
                () => new WebSocketClient());

            return manager;
        }
    }
}
