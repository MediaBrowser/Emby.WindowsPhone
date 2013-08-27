using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Services;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RemoteViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the RemoteViewModel class.
        /// </summary>
        public RemoteViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public bool IsLoading { get; set; }
        public bool SendingCommand { get; set; }
        public bool IsPinned { get; set; }
        public List<object> Clients { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    IsPinned = TileService.Current.TileExists(Constants.Pages.Remote.RemoteView);

                    await GetClients(false);
                });
            }
        }

        private async Task GetClients(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_dataLoaded && !isRefresh))
            {
                return;
            }


        }

        public RelayCommand PinCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsPinned)
                    {
                        // Unpin the tile
                        var tile = TileService.Current.GetTile(Constants.Pages.Remote.RemoteView);
                        tile.Delete();

                        IsPinned = false;
                    }
                    else
                    {
                        var tileData = new ShellTileServiceFlipTileData
                        {
                            Title = "MB Remote",
                            BackgroundImage = new Uri("/Assets/Tiles/MBRemoteTile.png", UriKind.Relative)
                        };

                        TileService.Current.Create(new Uri(Constants.Pages.Remote.RemoteView, UriKind.Relative), tileData, false);

                        IsPinned = true;
                    }
                });
            }
        }
    }
}