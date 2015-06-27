using System;
using System.Windows.Navigation;

namespace Emby.WindowsPhone
{
    public class EmbyUriMapper : UriMapperBase
    {
        private const string OldProtocol = "mediabrowser";
        private const string Protocol = "emby";
        private const string PhotoUploadProtocol = "ConfigurePhotosUploadSettings";

        public override Uri MapUri(Uri uri)
        {
            var url = uri.ToString().ToLower();
            if (url.Contains(Protocol) || url.Contains(OldProtocol))
            {
                return new Uri(Constants.Pages.SplashScreen, UriKind.Relative);
            }

            if (url.Contains(PhotoUploadProtocol))
            {
                return new Uri(string.Format(Constants.PhoneTileUrlFormat, "photouploadsettings", string.Empty, string.Empty), UriKind.Relative);
            }

            return uri;
        }
    }
}
