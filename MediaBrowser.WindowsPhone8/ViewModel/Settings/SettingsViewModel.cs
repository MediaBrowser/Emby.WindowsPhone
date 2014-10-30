﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Streaming;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Notification;
using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;
using LockScreenService = MediaBrowser.WindowsPhone.Services.LockScreenService;

namespace MediaBrowser.WindowsPhone.ViewModel.Settings
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IApplicationSettingsService _applicationSettings;

        public bool LoadingFromSettings;

        /// <summary>
        /// Initializes a new instance of the PushViewModel class.
        /// </summary>
        public SettingsViewModel(IConnectionManager connectionManager, INavigationService navigationService, IApplicationSettingsService applicationSettings)
            : base(navigationService, connectionManager)
        {
            _applicationSettings = applicationSettings;

            if (IsInDesignMode)
            {
                FoundServers = new ObservableCollection<ServerInfo>
                {
                    new ServerInfo{Id = Guid.NewGuid().ToString(), Name = "Home", LocalAddress = "http://192.168.0.2:8096"}
                };
            }
            else
            {
                LoadingFromSettings = true;
                SendTileUpdates = SendToastUpdates = true;
                RegisteredText = AppResources.DeviceNotRegistered;
                LoadingFromSettings = false;

                SetStreamingQuality();
            }
        }

        private void SetStreamingQuality()
        {
            StreamingResolution res;
            StreamingLMH lmh;
            App.SpecificSettings.StreamingQuality.BreakDown(out res, out lmh);

            StreamingResolution wifires;
            StreamingLMH wifilmh;
            App.SpecificSettings.StreamingQuality.BreakDown(out wifires, out wifilmh);

            StreamingResolutions = Enum<StreamingResolution>.GetNames();
            StreamingResolution = StreamingResolutions.FirstOrDefault(x => x == res);
            WifiStreamingResolution = StreamingResolutions.FirstOrDefault(x => x == wifires);

            SetQuality(lmh);
            SetQuality(wifilmh, true);
        }

        private void SetQuality(StreamingLMH lmh, bool isWifi = false)
        {
            if (isWifi)
            {
                WifiStreamingLmhs = Enum<StreamingLMH>.GetNames();
                WifiStreamingLmh = WifiStreamingLmhs.FirstOrDefault(x => x == lmh);
            }
            else
            {
                StreamingLmhs = Enum<StreamingLMH>.GetNames();
                StreamingLmh = StreamingLmhs.FirstOrDefault(x => x == lmh);
            }
        }

        public string RegisteredText { get; set; }
        public bool SendToastUpdates { get; set; }
        public bool SendTileUpdates { get; set; }

        public List<StreamingLMH> StreamingLmhs { get; set; }
        public StreamingLMH StreamingLmh { get; set; }
        public List<StreamingLMH> WifiStreamingLmhs { get; set; }
        public StreamingLMH WifiStreamingLmh { get; set; }
        public List<StreamingResolution> StreamingResolutions { get; set; }
        public StreamingResolution StreamingResolution { get; set; }
        public StreamingResolution WifiStreamingResolution { get; set; }

        public bool CanChangeQuality
        {
            get { return StreamingResolution != StreamingResolution.ThreeSixty; }
        }

        public bool CanChangeWifiQuality
        {
            get { return WifiStreamingResolution != StreamingResolution.ThreeSixty; }
        }

        [UsedImplicitly]
        private void OnStreamingLmhChanged()
        {
            App.SpecificSettings.StreamingQuality = StreamingResolution.ToStreamingQuality(StreamingLmh);
        }

        [UsedImplicitly]
        private void OnStreamingResolutionChanged()
        {
            App.SpecificSettings.StreamingQuality = StreamingResolution.ToStreamingQuality(StreamingLmh);
            StreamingLmhs = null;
            SetQuality(StreamingLmh);
        }

        [UsedImplicitly]
        private void OnWifiStreamingLmhChanged()
        {
            App.SpecificSettings.WifiStreamingQuality = WifiStreamingResolution.ToStreamingQuality(WifiStreamingLmh);
        }

        [UsedImplicitly]
        private void OnWifiStreamingResolutionChanged()
        {
            App.SpecificSettings.WifiStreamingQuality = WifiStreamingResolution.ToStreamingQuality(WifiStreamingLmh);
            WifiStreamingLmhs = null;
            SetQuality(WifiStreamingLmh, true);
        }

        public bool IsLockScreenProvider
        {
            get
            {
                return LockScreenService.Current.IsProvidedByCurrentApplication;
            }
        }

        public string LockScreenText
        {
            get
            {
                return IsLockScreenProvider ? AppResources.LabelIsLockScreenProvider : AppResources.LabelIsNotLockScreenProvider;
            }
        }

        public List<Stream> Posters { get; set; }
        public ObservableCollection<BaseItemDto> Folders { get; set; }
        public BaseItemDto SelectedCollection { get; set; }

        public RelayCommand MakeLockScreenProviderCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var result = await LockScreenService.Current.RequestAccessAsync();

                    if (result == LockScreenServiceRequestResult.Granted)
                    {
                        RaisePropertyChanged(() => IsLockScreenProvider);
                        RaisePropertyChanged(() => LockScreenText);
                    }
                });
            }
        }

        public RelayCommand<SelectionChangedEventArgs> CollectionChangedCommand
        {
            get
            {
                return new RelayCommand<SelectionChangedEventArgs>(async args =>
                {
                    if (args == null)
                    {
                        return;
                    }

                    var collection = (BaseItemDto) args.AddedItems[0];
                    App.SpecificSettings.LockScreenCollectionId = collection.Id;
                    SelectedCollection = Folders.FirstOrDefault(x => x.Id == App.SpecificSettings.LockScreenCollectionId);
                    await LockScreenService.Current.SetLockScreen(App.SpecificSettings.LockScreenType);
                });
            }
        } 

        private void LoadPosterStreams()
        {
            var list = new List<Stream>();

            for (var i = 1; i <= 5; i++)
            {
                var file = string.Format("Images/folder{0}.jpg", i);
                var sri = Application.GetResourceStream(new Uri(file, UriKind.Relative));
                if (sri != null)
                {
                    list.Add(sri.Stream);
                }
            }

            Posters = list;
        }

        private void GetFolders()
        {
            var main = SimpleIoc.Default.GetInstance<MainViewModel>();
            var folders = main.Folders;
            if (folders.IsNullOrEmpty())
            {
                return;
            }

            Folders = folders;
            SelectedCollection = Folders.FirstOrDefault(x => x.Id == App.SpecificSettings.LockScreenCollectionId);
        }

        public RelayCommand SettingsPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LoadPosterStreams();
                    GetFolders();
                });
            }
        }

        public RelayCommand TestConnectionCommand
        {
            get
            {
                return new RelayCommand(async () => await TestConnection());
            }
        }

        private async Task TestConnection()
        {
            SetProgressBar(AppResources.SysTrayAuthenticating);

            if (NavigationService.IsNetworkAvailable)
            {
                Log.Info("Testing connection");


                var hostnameType = Uri.CheckHostName(App.Settings.ConnectionDetails.HostName);
                if (hostnameType == UriHostNameType.Unknown)
                {
                    MessageBox.Show(AppResources.ErrorInvalidHostname, AppResources.ErrorTitle, MessageBoxButton.OK);
                    return;
                }

                var result = await ConnectionManager.Connect(App.Settings.ConnectionDetails.ServerAddress, default(CancellationToken));

                if (await Utils.GetServerConfiguration(ApiClient, Log))
                {
                    if (!IsInDesignMode)
                    {
                        _applicationSettings.Reset(Constants.Settings.SelectedUserSetting);
                        SettingsSet(Constants.Settings.ConnectionSettings, App.Settings.ConnectionDetails);
                    }

                    SetProgressBar(AppResources.SysTrayAuthenticating);
                    await Utils.CheckProfiles(NavigationService, Log, ApiClient);
                }
                else
                {
                    Log.Info("Invalid connection details");
                    App.ShowMessage(AppResources.ErrorConnectionDetailsInvalid);
                }
            }

            SetProgressBar();
        }


        #region Server Broadcast code WP8 only

        public ObservableCollection<ServerInfo> FoundServers { get; set; }

        public RelayCommand FindServerLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    // If we're not connected to wifi or ethernet then we don't want to attempt this
                    if (!NavigationService.IsNetworkAvailable ||
                        (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet
                         && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211))
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayFindingServer);

                    Log.Info("Sending UDP broadcast");
                    var servers = await ConnectionManager.GetAvailableServers(default(CancellationToken));
                    FoundServers = new ObservableCollection<ServerInfo>(servers);
                    
                    SetProgressBar();
                });
            }
        }

        public RelayCommand<ServerInfo> ServerTappedCommand
        {
            get
            {
                return new RelayCommand<ServerInfo>(async server =>
                {
                    var address = new Uri(server.LocalAddress);
                    App.Settings.ConnectionDetails.HostName = address.Host;
                    App.Settings.ConnectionDetails.PortNo = address.Port;
                    //NavigationService.GoBack();

                    SetProgressBar(AppResources.SysTrayAuthenticating);

                    var result = await ConnectionManager.Connect(server, default(CancellationToken));

                    if (result.State == ConnectionState.Unavailable)
                    {
                        Log.Info("Invalid connection details");
                        App.ShowMessage(AppResources.ErrorConnectionDetailsInvalid);
                    }

                    if (result.State == ConnectionState.ServerSignIn)
                    {
                        await Utils.CheckProfiles(NavigationService, Log, result.ApiClient);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand GoToManualServerCommand
        {
            get
            {
                return new RelayCommand(() => NavigationService.NavigateTo(Constants.Pages.SettingsViewConnection));
            }
        }

        #endregion

        internal string DeviceId
        {
            get
            {
                var id = SimpleIoc.Default.GetInstance<IUserExtendedPropertiesService>().AnonymousUserID;
                return string.IsNullOrEmpty(id) ? "emulator" : id;
            }
        }

        [UsedImplicitly]
        private static string ParseANID(string anid)
        {
            if (!string.IsNullOrEmpty(anid))
            {
                var parts = anid.Split('&');
                var pairs = parts.Select(part => part.Split('='));
                var id = pairs
                    .Where(pair => pair.Length == 2 && pair[0] == "A")
                    .Select(pair => pair[1])
                    .FirstOrDefault();
                return id;
            }
            return "emulator";
        }

        private void SettingsSet(string key, object value)
        {
            if (!IsInDesignMode)
            {
                _applicationSettings.Set(key, value);
                _applicationSettings.Save();
            }
        }
    }
}