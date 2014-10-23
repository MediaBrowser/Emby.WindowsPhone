using System;
using System.Diagnostics;
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

        private bool _ignoreChange;

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
                _ignoreChange = true;
                IsPhotoUploadsEnabled = App.SpecificSettings.IsPhotoUploadsEnabled;
                UploadAll = App.SpecificSettings.UploadAllPhotos;
                AfterDateTime = App.SpecificSettings.UploadAfterDateTime;
                _ignoreChange = false;
            }
        }

        public bool IsPhotoUploadsEnabled { get; set; }

        public bool UploadAll { get; set; }
        public DateTime AfterDateTime { get; set; }

        [UsedImplicitly]
        private async void OnIsPhotoUploadsEnabledChanged()
        {
            App.SpecificSettings.IsPhotoUploadsEnabled = IsPhotoUploadsEnabled;
            await AddRemoveBackgroundTask();
        }

        [UsedImplicitly]
        private void OnUploadAllChanged()
        {
            App.SpecificSettings.UploadAllPhotos = UploadAll;
        }

        [UsedImplicitly]
        private void OnAfterDateTimeChanged()
        {
            App.SpecificSettings.UploadAfterDateTime = AfterDateTime;
        }

        private async Task AddRemoveBackgroundTask()
        {
            if (IsInDesignMode || _ignoreChange) return;

            if (IsPhotoUploadsEnabled)
            {
                BackgroundTaskService.Current.CreateTask();
                LaunchBackgroundTask();
            }
            else
            {
                BackgroundTaskService.Current.RemoveTask();
            }
        }

        [Conditional("DEBUG")]
        private void LaunchBackgroundTask()
        {
            BackgroundTaskService.Current.LaunchTask();
        }
    }
}