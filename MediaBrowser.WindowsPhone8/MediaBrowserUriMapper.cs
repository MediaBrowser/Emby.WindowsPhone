using System;
using System.Windows.Navigation;

namespace MediaBrowser.WindowsPhone
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
                
            }

            return uri;
        }
    }
}
