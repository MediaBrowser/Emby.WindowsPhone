using System.Security.Cryptography;
using System.Text;

namespace MediaBrowser
{
    public static class ExtensionMethods
    {
        internal static byte[] ToHash(this string pinCode)
        {
            var sha1 = new SHA1Managed();
            var encoding = new UTF8Encoding();
            sha1.ComputeHash(encoding.GetBytes(pinCode));

            return sha1.Hash;
        }
    }
}
