using System;
using System.Diagnostics;
using System.Linq;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Logging;
using MediaBrowser.Windows8.Model;
using MetroLog;
using Windows.Networking.Connectivity;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace MediaBrowser.Windows8
{
    public static class ExtensionMethods
    {
        public static byte[] ToHash(this string s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var buff = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var array = new byte[0];
            CryptographicBuffer.CopyToByteArray(hashed, out array);
            return array;
        }

        public static GridItemWrapper<BaseItemDto> ToWrapper(this BaseItemDto item, int rowSpan = 1, int colSpan = 1)
        {
            return new GridItemWrapper<BaseItemDto>(item) { ColSpan = colSpan, RowSpan = rowSpan };
        }

        internal static ExtendedApiClient SetDeviceProperties(this ExtendedApiClient apiClient)
        {
            var hostNames = NetworkInformation.GetHostNames();
            var localName = hostNames.FirstOrDefault(name => name.DisplayName.Contains(".local"));
            var computerName = localName.DisplayName.Replace(".local", "");
            Debug.WriteLine(computerName);
            try
            {
                apiClient.DeviceName = computerName.Substring(0, computerName.IndexOf(".", StringComparison.Ordinal));
            }
            catch
            {
                apiClient.DeviceName = computerName;
            }
            apiClient.DeviceId = GetHardwareId();

            return apiClient;
        }

        private static string GetHardwareId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var id = token.Id;
            var reader = DataReader.FromBuffer(id);
            var bytes = new byte[id.Length];
            reader.ReadBytes(bytes);
            return BitConverter.ToString(bytes);
        }

        internal static LogLevel ToLogLevel(this LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    return LogLevel.Debug;
                case LogSeverity.Error:
                    return LogLevel.Error;
                case LogSeverity.Fatal:
                    return LogLevel.Fatal;
                case LogSeverity.Info:
                    return LogLevel.Info;
                case LogSeverity.Warn:
                    return LogLevel.Warn;
            }
        }
    }
}