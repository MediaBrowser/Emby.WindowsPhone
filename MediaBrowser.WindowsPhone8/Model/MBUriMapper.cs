using System;
using System.Windows.Navigation;

namespace MediaBrowser.WindowsPhone.Model
{
    public class MBUriMapper : UriMapperBase
    {
        public override Uri MapUri(Uri uri)
        {
            return uri;
        }
    }
}
