using System.Collections.Generic;
using System.Globalization;
using Cimbalino.Toolkit.Services;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dlna.Profiles;
using System.Xml.Serialization;

namespace MediaBrowser.Dlna.Profiles
{
    [XmlRoot("Profile")]
    public class WindowsPhoneProfile : DefaultProfile
    {
        public WindowsPhoneProfile()
        {
            Name = "Windows Phone";
        }
    }
}