using System;

namespace MediaBrowser.Windows8.Model
{
    public interface INavigationService
    {
        void Navigate(Type source, object parameter = null);
        void Navigate<T>(object parameter = null);

        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void GoBack();
        void GoForward();

        bool IsNetworkAvailable { get; }

        //IView CurrentView { get; }
    }
}