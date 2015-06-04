using System.Xml.Serialization;
using MediaBrowser.Model.Dlna;

namespace MediaBrowser.Dlna.Profiles
{
    [XmlRoot("Profile")]
    public class WindowsPhoneProfile : DeviceProfile
    {
        public WindowsPhoneProfile()
        {
            Name = "Windows Phone";
        }
    }
}