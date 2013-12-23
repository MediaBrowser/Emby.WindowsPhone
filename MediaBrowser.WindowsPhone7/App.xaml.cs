using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using Cimbalino.Phone.Toolkit.Helpers;
using Cimbalino.Phone.Toolkit.Services;
using Coding4Fun.Toolkit.Controls;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.ApiInteraction.WebSocket;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.ViewModel;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MediaBrowser.Model;

using ScottIsAFool.WindowsPhone.Logging;
using Telerik.Windows.Controls;

namespace MediaBrowser.WindowsPhone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly ILog _logger;

        private static SettingsService _settings;
        public static SettingsService Settings
        {
            get { return _settings ?? (_settings = (SettingsService)Current.Resources["AppSettings"]); }
        }

        private static SpecificSettings _specificSettings;
        public static SpecificSettings SpecificSettings
        {
            get { return _specificSettings ?? (_specificSettings = (SpecificSettings)Current.Resources["SpecificSettings"]); }
        }

        public static object SelectedItem { get; set; }

        public static ApiWebSocket WebSocketClient { get; set; }

        public static void ShowMessage(string message, string title = "", Action action = null)
        {
            var prompt = new ToastPrompt
            {
                Title = title,
                Message = message,
                Foreground = new SolidColorBrush(Colors.White),
                TextWrapping = TextWrapping.Wrap
            };

            if (action != null)
                prompt.Tap += (s, e) => action();
            prompt.Show();
        }

        // Easy access to the root frame
        public static PhoneApplicationFrame RootFrame
        {
            get;
            private set;
        }

        // Constructor
        public App()
        {
            _logger = new WPLogger(typeof(App));

            // Global handler for uncaught exceptions. 
            UnhandledException += ApplicationUnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            ThemeManager.OverrideTheme(Theme.Dark);
            ThemeManager.SetAccentColor(Colors.Green);
            WPLogger.AppVersion = ApplicationManifest.Current.App.Version;
            WPLogger.LogConfiguration.LogType = LogType.WriteToFile;
            WPLogger.LogConfiguration.LoggingIsEnabled = true;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disable user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
            AuthenticationService.Current.Start(SimpleIoc.Default.GetInstance<IExtendedApiClient>());
            ApplicationUsageHelper.Init(ApplicationManifest.Current.App.Version);
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
            {
                AuthenticationService.Current.Start(SimpleIoc.Default.GetInstance<IExtendedApiClient>());
            }

            ApplicationUsageHelper.OnApplicationActivated();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var ast = SimpleIoc.Default.GetInstance<IApplicationSettingsService>();
            ast.Set(Constants.Settings.SpecificSettings, SpecificSettings);
            ast.Save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            SaveSettings();
            ViewModelLocator.Cleanup();
        }

        // Code to execute if a navigation fails
        private void RootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            _logger.FatalException(string.Format("NavigationFailed, URI: {0}", e.Uri), e.Exception);
        }

        // Code to execute on Unhandled Exceptions
        private void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            _logger.FatalException("UnhandledException", e.ExceptionObject);

            e.Handled = true;
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool _phoneApplicationInitialized;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (_phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame
                            {
                                Background = new SolidColorBrush(Colors.Transparent)
                            };
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrameNavigationFailed;

            // Ensure we don't initialize again
            _phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
