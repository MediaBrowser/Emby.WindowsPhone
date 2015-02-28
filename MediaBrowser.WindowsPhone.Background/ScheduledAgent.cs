using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction;
using MediaBrowser.ApiInteraction.Sync;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Session;
using MediaBrowser.WindowsPhone.Logging;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Connection;
using MediaBrowser.WindowsPhone.Model.Photo;
using MediaBrowser.WindowsPhone.Model.Security;
using Microsoft.Phone.Scheduler;
using ScottIsAFool.WindowsPhone.Logging;

namespace MediaBrowser.WindowsPhone.Background
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static IApiClient _apiClient;
        private static readonly ILogger MediaBrowserLogger = new MBLogger(typeof(ScheduledAgent));
        private static readonly IApplicationSettingsService ApplicationSettings = new ApplicationSettingsService();
        private static ContentUploader _contentUploader;
        private static ILog _logger;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            _logger = new WPLogger(GetType());
            if(_apiClient == null) CreateClient();
            WPLogger.AppVersion = ApplicationManifest.Current.App.Version;
            WPLogger.LogConfiguration.LogType = LogType.WriteToFile;
            WPLogger.LogConfiguration.LoggingIsEnabled = true;

            if (_apiClient != null)
            {
                _contentUploader = new ContentUploader(_apiClient, MediaBrowserLogger);
            }

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

        private static void CreateClient()
        {
            try
            {
                _logger.Info("Creating API Client");
                var device = new Device { DeviceId = SharedUtils.GetDeviceId(), DeviceName = SharedUtils.GetDeviceName() };
                var server = ApplicationSettings.Get<ServerInfo>(Constants.Settings.DefaultServerConnection);
                if (server == null)
                {
                    _logger.Info("No server details found");
                    return;
                }

                var serverAddress = server.LastConnectionMode.HasValue && server.LastConnectionMode.Value == ConnectionMode.Manual ? server.ManualAddress : server.RemoteAddress;
                var client = new ApiClient(MediaBrowserLogger, serverAddress, "Windows Phone 8", device, ApplicationManifest.Current.App.Version, new CryptographyProvider());
                client.SetAuthenticationInfo(server.AccessToken, server.UserId);

                _logger.Info("Client created");
                _apiClient = client;
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
            _logger.Info("Task started");
            var uploadTask = task as ResourceIntensiveTask;
            if (uploadTask == null)
            {
                _logger.Info("Not a photo upload task");
                NotifyComplete();
                return;
            }

            if (_contentUploader == null)
            {
                _logger.Info("ContentUploader is null, nothing to do");
                NotifyComplete();
                return;
            }

            _logger.Info("Getting upload settings");
            var uploadSettings = ApplicationSettings.Get<UploadSettings>(Constants.Settings.PhotoUploadSettings);

            if (uploadSettings == null || !uploadSettings.IsPhotoUploadsEnabled)
            {
                if(uploadSettings == null) _logger.Info("No upload settings found");
                else if(!uploadSettings.IsPhotoUploadsEnabled) _logger.Info("Photo uploads not enabled");

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
                _logger.Info("Start upload");
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