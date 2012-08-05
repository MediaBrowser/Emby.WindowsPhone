using System;
using System.Windows.Navigation;
using MediaBrowser.Model.Entities;

namespace MediaBrowser.Model
{
    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;
        void NavigateTo(Uri pageUri);
        void NavigateTo(string pageUri);
        void GoBack();
        bool IsNetworkAvailable { get; }
        void NavigateToPage(string link);
        void NavigateTopage(ApiBaseItemWrapper<ApiBaseItem> item);
    }
}
