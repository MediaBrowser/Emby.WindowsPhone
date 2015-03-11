using System;
using Ailon.WP.Utils;
using Cimbalino.Toolkit.Services;
using Cimbalino.Toolkit.Helpers;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Session;
using MediaBrowser.WindowsPhone.CimbalinoToolkit;
using MediaBrowser.WindowsPhone.Model.Connection;
using MediaBrowser.WindowsPhone.Model.Security;
using Microsoft.Phone.Info;

namespace MediaBrowser.WindowsPhone
{
    public static class SharedUtils
    {
        public static void CopyItem<T>(this T source, T destination) where T : class
        {
            foreach (var sourcePropertyInfo in source.GetType().GetProperties())
            {
                var destPropertyInfo = source.GetType().GetProperty(sourcePropertyInfo.Name);

                destPropertyInfo.SetValue(
                    destination,
                    sourcePropertyInfo.GetValue(source, null),
                    null);
            }
        }

        public static string GetDeviceName()
        {
            var deviceName = DeviceStatus.DeviceName;
            var deviceId = DeviceStatus.DeviceManufacturer;
            var phone = PhoneNameResolver.Resolve(deviceId, deviceName);
            var deviceInfo = String.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

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
