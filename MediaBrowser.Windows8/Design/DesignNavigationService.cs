using System;
using MediaBrowser.Windows8.Model;
using Windows.Networking.Connectivity;

namespace MediaBrowser.Windows8.Design
{
    public class DesignNavigationService : INavigationService
    {
        public void Navigate(Type source, object parameter = null)
        {
        }

        public void Navigate<T>(object parameter = null)
        {
        }

        public bool CanGoBack
        {
            get { return false; }
        }

        public bool CanGoForward
        {
            get { return false; }
        }

        public void GoBack()
        {
            
        }

        public void GoForward()
        {
            
        }

        public bool IsNetworkAvailable
        {
            get { return false; }
        }
    }
}
