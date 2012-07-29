using GalaSoft.MvvmLight;
using MediaBrowser.Model;

namespace MediaBrowser.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class FolderViewModel : ViewModelBase
    {
        private INavigationService NavService;
        /// <summary>
        /// Initializes a new instance of the FolderViewModel class.
        /// </summary>
        public FolderViewModel(INavigationService navService)
        {
            if (IsInDesignMode)
            {

            }
            else
            {
                NavService = navService;
            }
        }
    }
}