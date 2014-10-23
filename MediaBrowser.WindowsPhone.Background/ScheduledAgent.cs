using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model;
using MediaBrowser.Model.Devices;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Photo;
using Microsoft.Phone.Scheduler;
using Microsoft.Xna.Framework.Media;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Background
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private readonly IExtendedApiClient _apiClient;
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
            _apiClient = CreateClient();
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

        private IExtendedApiClient CreateClient()
        {
            try
            {
                var applicationSettings = new ApplicationSettingsService();
                var connectionDetails = applicationSettings.Get<ConnectionDetails>(Constants.Settings.ConnectionSettings);
                var device = new Device { DeviceId = SharedUtils.GetDeviceId(), DeviceName = SharedUtils.GetDeviceName() };
                var client = new ExtendedApiClient(_mediaBrowserLogger, connectionDetails.ServerAddress, "Windows Phone 8", device, ApplicationManifest.Current.App.Version, new ClientCapabilities { SupportsContentUploading = true });

                AuthenticationService.Current.Start(client);

                return client;
            }
            catch (Exception ex)
            {
                _logger.FatalException("Error creating ApiClient", ex);
                return null;
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