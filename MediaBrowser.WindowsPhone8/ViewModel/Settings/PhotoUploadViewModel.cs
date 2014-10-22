using System.Threading.Tasks;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.Settings
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PhotoUploadViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the PhotoUploadViewModel class.
        /// </summary>
        public PhotoUploadViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                IsPhotoUploadsEnabled = true;
            }
            else
            {
                IsPhotoUploadsEnabled = App.SpecificSettings.IsPhotoUploadsEnabled;
            }
        }

        public bool IsPhotoUploadsEnabled { get; set; }

        [UsedImplicitly]
        private async void OnIsPhotoUploadsEnabledChanged()
        {
            App.SpecificSettings.IsPhotoUploadsEnabled = IsPhotoUploadsEnabled;
            await AddRemoveBackgroundTask();
        }

        private async Task AddRemoveBackgroundTask()
        {
            if (IsInDesignMode) return;

            if (IsPhotoUploadsEnabled)
            {
                BackgroundTaskService.Current.CreateTask();
            }
            else
            {
                BackgroundTaskService.Current.RemoveTask();
            }
        }
    }
}