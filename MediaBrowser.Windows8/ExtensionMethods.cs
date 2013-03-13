using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace MediaBrowser.Windows8
{
    public static class ExtensionMethods
    {
        public static string ToHash(this string s)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var buff = CryptographicBuffer.ConvertStringToBinary(s, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}