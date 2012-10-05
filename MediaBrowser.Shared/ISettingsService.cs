using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.DTO;

namespace MediaBrowser.Model
{
    public interface ISettingsService
    {
        DtoUser LoggedInUser { get; set; }
        string PinCode { get; set; }
        string HostName { get; set; }
        int PortNo { get; set; }
        string ApiUrl { get; }
        bool CheckHostAndPort();
        ServerConfiguration ServerConfiguration { get; set; }
    }
}
