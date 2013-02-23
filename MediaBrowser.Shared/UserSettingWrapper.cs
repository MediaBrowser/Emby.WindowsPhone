using MediaBrowser.Model.Dto;

namespace MediaBrowser.Shared
{
    public class UserSettingWrapper
    {
        public UserDto User { get; set; }
        public string Pin { get; set; }
    }
}
