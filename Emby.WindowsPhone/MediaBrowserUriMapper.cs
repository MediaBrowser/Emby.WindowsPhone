using System;
using System.Windows.Navigation;

namespace Emby.WindowsPhone
{
    public class MediaBrowserUriMapper : UriMapperBase
    {
        private const string Protocol = "mediabrowser";
        private const string PhotoUploadProtocol = "ConfigurePhotosUploadSettings";

        public override Uri MapUri(Uri uri)
        {
            if (uri.ToString().ToLower().Contains(Protocol))
            {
                return new Uri(Constants.Pages.SplashScreen, UriKind.Relative);
            }

            if (uri.ToString().Contains(PhotoUploadProtocol))
            {
                return new Uri(string.Format(Constants.PhoneTileUrlFormat, "photouploadsettings", string.Empty, string.Empty), UriKind.Relative);
            }

            return uri;
        }
    }
}
