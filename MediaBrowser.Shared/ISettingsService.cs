using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.DTO;
using MediaBrowser.Model.System;

namespace MediaBrowser.Model
{
    public interface ISettingsService
    {
        DtoUser LoggedInUser { get; set; }
        string PinCode { get; set; }
        ConnectionDetails ConnectionDetails { get; set; }
        ServerConfiguration ServerConfiguration { get; set; }
        SystemInfo SystemStatus { get; set; }
    }
}
