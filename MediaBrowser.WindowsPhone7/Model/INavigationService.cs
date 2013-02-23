using System;
using System.Windows.Navigation;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Model
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void NavigateTo(Uri pageUri);
        void NavigateTo(string pageUri);
        void GoBack();
        bool IsNetworkAvailable { get; }
        void NavigateToPage(string link);
        void NavigateToPage(BaseItemDto item);
    }
}
