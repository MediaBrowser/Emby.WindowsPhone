using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.System;
using MediaBrowser.Services;
using PropertyChanged;

namespace MediaBrowser.Model
{
    [ImplementPropertyChanged]
    public class SettingsService : ISettingsService
    {
        public UserDto LoggedInUser
        {
            get
            {
                return AuthenticationService.Current.LoggedInUser;
            }
        }

        public ConnectionDetails ConnectionDetails { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public SystemInfo SystemStatus { get; set; }
        public bool SupportsLiveTv { get; set; }
    }
}
