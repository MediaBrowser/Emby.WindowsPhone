using MediaBrowser.Model.Dto;
using PropertyChanged;

namespace MediaBrowser.Shared
{
    [ImplementPropertyChanged]
    public class UserSettingWrapper
    {
        public UserDto User { get; set; }
        public string Pin { get; set; }
    }
}
