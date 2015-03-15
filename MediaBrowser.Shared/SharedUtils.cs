using System;
using Ailon.WP.Utils;
using MediaBrowser.WindowsPhone.CimbalinoToolkit;
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
            var deviceInfo = String.Format("{0} ({1})", phone.CanonicalModel, phone.CanonicalManufacturer);

            return deviceInfo;
        }

        public static string GetDeviceId()
        {
            var uniqueId = new DeviceExtendedPropertiesService().DeviceUniqueId;
            var deviceId = Convert.ToBase64String(uniqueId, 0, uniqueId.Length);

            return deviceId;
        }
    }
}
