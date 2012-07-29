using MediaBrowser.Model.Users;

namespace MediaBrowser.Model
{
    public interface ISettingsService
    {
        User LoggedInUser { get; set; }
        string HostName { get; set; }
        string PortNo { get; set; }
        string ApiUrl { get; }
        bool CheckHostAndPort();
    }
}
