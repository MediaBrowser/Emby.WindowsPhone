using System;
using System.Windows.Navigation;

namespace MediaBrowser.WindowsPhone
{
    public class MediaBrowserUriMapper : UriMapperBase
    {
        private const string Protocol = "mediabrowser";

        public override Uri MapUri(Uri uri)
        {
            if (uri.ToString().ToLower().Contains(Protocol))
            {
                return new Uri(Constants.Pages.SplashScreen, UriKind.Relative);
            }

            return uri;
        }
    }
}
