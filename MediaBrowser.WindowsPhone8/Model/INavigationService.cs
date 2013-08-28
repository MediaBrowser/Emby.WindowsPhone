using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Model
{
    public interface INavigationService : Cimbalino.Phone.Toolkit.Services.INavigationService
    {
        bool IsNetworkAvailable { get; }
        void NavigateTo(BaseItemDto item);
    }
}
