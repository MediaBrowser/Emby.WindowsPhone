using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Logging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Services;
using Microsoft.Phone.Scheduler;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Background
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private IApiClient _apiClient;
        private readonly ILogger _mediaBrowserLogger = new MBLogger(typeof(ScheduledAgent));
        private readonly IApplicationSettingsService _applicationSettings = new ApplicationSettingsService();
        private static ContentUploader _contentUploader;
        private static ILog _logger;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            _logger = new WPLogger(GetType());
            CreateClient();
            WPLogger.AppVersion = ApplicationManifest.Current.App.Version;
            WPLogger.LogConfiguration.LogType = LogType.WriteToFile;
            WPLogger.LogConfiguration.LoggingIsEnabled = true;

            _contentUploader = new ContentUploader(_apiClient, _mediaBrowserLogger);

            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        private async void CreateClient()
        {
            try
            {
                var device = new Device { DeviceId = SharedUtils.GetDeviceId(), DeviceName = SharedUtils.GetDeviceName() + " Audio Player" };
                var manager = SharedUtils.CreateConnectionManager(device, _mediaBrowserLogger);
                await manager.Connect(default(CancellationToken)).ContinueWith(task =>
                {
                    var auth = new AuthenticationService(manager);
                    auth.Start();

                    _apiClient = manager.CurrentApiClient;
                });
            }
            catch (Exception ex)
            {
                _logger.FatalException("Error creating ApiClient", ex);
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override async void OnInvoke(ScheduledTask task)
        {
            var uploadTask = task as ResourceIntensiveTask;
            if (uploadTask == null)
            {
                NotifyComplete();
                return;
            }

            var uploadSettings = _applicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);

            if (uploadSettings == null || !uploadSettings.IsPhotoUploadsEnabled)
            {
                NotifyComplete();
                return;
            }

            var device = _apiClient.Device as Device;
            if (device != null)
            {
                device.UploadAll = uploadSettings.UploadAllPhotos;
                device.AfterDateTime = uploadSettings.UploadAfterDateTime;
            }

            try
            {
                await _contentUploader.UploadImages(new Progress<double>(), CancellationToken.None);
            }
            catch (HttpException ex)
            {
                _logger.ErrorException("Error Uploading Images (" + ex.StatusCode + ")", ex);
            }

            NotifyComplete();
        }
    }
}