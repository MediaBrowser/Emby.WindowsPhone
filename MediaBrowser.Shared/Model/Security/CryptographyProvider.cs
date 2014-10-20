using System.Security.Cryptography;
using MediaBrowser.ApiInteraction.Cryptography;

namespace MediaBrowser.WindowsPhone.Model.Security
{
    public class CryptographyProvider : ICryptographyProvider
    {
        public byte[] CreateSha1(byte[] value)
        {
            var sha1 = new SHA1Managed();
            sha1.ComputeHash(value);

            return sha1.Hash;
        }

        public byte[] CreateMD5(byte[] value)
        {
            var md5 = new MD5Managed();
            md5.ComputeHash(value);
            return md5.Hash;
        }
    }
}
