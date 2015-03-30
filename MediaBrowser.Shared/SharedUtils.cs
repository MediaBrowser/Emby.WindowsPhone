using System;
using Ailon.WP.Utils;
using MediaBrowser.ApiInteraction.Cryptography;
using MediaBrowser.WindowsPhone.CimbalinoToolkit;
using MediaBrowser.WindowsPhone.Model.Security;
using Microsoft.Phone.Info;

namespace MediaBrowser.WindowsPhone
{
    public static class SharedUtils
    {
        private static ICryptographyProvider _crypt = new CryptographyProvider();

        public static void CopyItem<T>(this T source, T destination) where T : class
        {
            foreach (var sourcePropertyInfo in source.GetType().GetProperties())
            {
                var destPropertyInfo = source.GetType().GetProperty(sourcePropertyInfo.Name);

                if (destPropertyInfo.CanWrite)
                {
                    destPropertyInfo.SetValue(
                        destination,
                        sourcePropertyInfo.GetValue(source, null),
                        null);
                }
            }
        }

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
            var hashedId = _crypt.CreateMD5(uniqueId);
            
            return new Guid(hashedId).ToString();
        }
    }
}
