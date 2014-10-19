using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;

namespace MediaBrowser.WindowsPhone.Model.Interfaces
{
    public interface INavigationService : Cimbalino.Phone.Toolkit.Services.INavigationService
    {
        bool IsNetworkAvailable { get; }
        bool IsOnWifi { get; }
        void NavigateTo(BaseItemDto item);
        void NavigateTo(BaseItemInfo item);
    }
}
