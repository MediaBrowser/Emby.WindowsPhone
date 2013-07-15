using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MediaBrowser.Windows8.Common;
using MediaBrowser.Windows8.Model;
using System;
using MediaBrowser.Windows8.Views;
using MetroLog;
using MetroLog.Targets;
using WinRtUtility;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MediaBrowser.Model.Dto;
using Windows.UI.Xaml.Media;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace MediaBrowser.Windows8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static SettingsService _settings;
        public static SettingsService Settings
        {
            get { return _settings ?? (_settings = (SettingsService)Current.Resources["AppSettings"]); }
        }

        public static MediaElement MediaElement
        {
            get
            {
                var rootGrid = VisualTreeHelper.GetChild(Window.Current.Content, 0);
                return (MediaElement)VisualTreeHelper.GetChild(rootGrid, 0);
            }
        }

        public static Window ThisWindow { get; private set; }

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            LogManagerFactory.DefaultConfiguration.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FileStreamingTarget
                                                                                                 {
                                                                                                     RetainDays= 10
                                                                                                 });

            GlobalCrashHandler.Configure();

            ThisWindow = Window.Current;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame
                {
                    Style = Resources["RootFrameStyle"] as Style
                };
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
                var nav = SimpleIoc.Default.GetInstance<NavigationService>();
                nav.Frame = rootFrame;
            }
            
            if (ViewModelBase.IsInDesignModeStatic)
            {
                Settings.LoggedInUser = new UserDto
                                            {
                                                Id = new Guid("5d1cf7fce25943b790d140095457a42b").ToString(),
                                                Name = "ScottIsAFool"
                                            };
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(LoadingView)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
            SettingsCharm.Create();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            var settingSaver = new ObjectStorageHelper<SpecificSettings>(StorageType.Roaming);
            await settingSaver.SaveAsync(SimpleIoc.Default.GetInstance<SpecificSettings>(), Constants.Settings.SpecificSettings);
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(LoadingView), args.QueryText);
            Window.Current.Content = frame;

            var nav = SimpleIoc.Default.GetInstance<NavigationService>();
            nav.Frame = frame;

            // Ensure the current window is active
            Window.Current.Activate();
            SettingsCharm.Create();
        }
    }
}
