using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.System;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.Model
{
    public interface ISettingsService
    {
        UserDto LoggedInUser { get; }
        ConnectionDetails ConnectionDetails { get; set; }
        ServerConfiguration ServerConfiguration { get; set; }
        PublicSystemInfo SystemStatus { get; set; }
        LiveTvInfo LiveTvInfo { get; set; }
    }
}
