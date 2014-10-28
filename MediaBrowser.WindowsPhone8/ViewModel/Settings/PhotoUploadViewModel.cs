using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Services;

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
        private bool _ignoreChange;

        /// <summary>
        /// Initializes a new instance of the PhotoUploadViewModel class.
        /// </summary>
        public PhotoUploadViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            if (IsInDesignMode)
            {
                IsPhotoUploadsEnabled = true;
            }
            else
            {
                _ignoreChange = true;
                IsPhotoUploadsEnabled = App.UploadSettings.IsPhotoUploadsEnabled;
                UploadAll = App.UploadSettings.UploadAllPhotos;
                AfterDateTime = App.UploadSettings.UploadAfterDateTime;
                _ignoreChange = false;
            }
        }

        public bool IsPhotoUploadsEnabled { get; set; }

        public bool UploadAll { get; set; }
        public DateTime AfterDateTime { get; set; }

        [UsedImplicitly]
        private async void OnIsPhotoUploadsEnabledChanged()
        {
            App.UploadSettings.IsPhotoUploadsEnabled = IsPhotoUploadsEnabled;
            await AddRemoveBackgroundTask();
        }

        [UsedImplicitly]
        private void OnUploadAllChanged()
        {
            App.UploadSettings.UploadAllPhotos = UploadAll;
        }

        [UsedImplicitly]
        private void OnAfterDateTimeChanged()
        {
            App.UploadSettings.UploadAfterDateTime = AfterDateTime;
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