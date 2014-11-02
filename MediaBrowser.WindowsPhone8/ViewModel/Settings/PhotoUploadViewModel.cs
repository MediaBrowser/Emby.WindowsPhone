using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Cimbalino.Phone.Toolkit.Services;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.WindowsPhone.Services;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;

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
        private readonly IApplicationSettingsService _appSettingsService;
        private bool _ignoreChange;

        /// <summary>
        /// Initializes a new instance of the PhotoUploadViewModel class.
        /// </summary>
        public PhotoUploadViewModel(INavigationService navigationService, IConnectionManager connectionManager, IApplicationSettingsService appSettingsService)
            : base(navigationService, connectionManager)
        {
            _appSettingsService = appSettingsService;
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

                _appSettingsService.Set(Constants.Settings.PhotoUploadSettings, App.UploadSettings);
                _appSettingsService.Save();

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