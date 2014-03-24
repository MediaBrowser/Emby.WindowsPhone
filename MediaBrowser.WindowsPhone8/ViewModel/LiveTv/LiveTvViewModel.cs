using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone.ViewModel;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LiveTvViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _upcomingLoaded;
        private bool _whatsOnLoaded;

        private DateTime? _upcomingLastRun;
        private DateTime? _whatsOnLastRun;

        private readonly string _tileUrl = string.Format(Constants.PhoneTileUrlFormat, "LiveTV", string.Empty, "Live TV");

        /// <summary>
        /// Initializes a new instance of the LiveTvViewModel class.
        /// </summary>
        public LiveTvViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public List<ProgramInfoDto> WhatsOn { get; set; }
        public List<ProgramInfoDto> Upcoming { get; set; }

        public bool ShowMoreWhatsOn { get; set; }
        public bool ShowMoreUpcoming { get; set; }
        public bool IsPinned { get; set; }

        public RelayCommand LiveTvPageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    IsPinned = TileService.Current.TileExists(_tileUrl);
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        public RelayCommand<bool> MoreCommand
        {
            get
            {
                return new RelayCommand<bool>(isWhatsOn =>
                {
                    if (SimpleIoc.Default.GetInstance<AllProgrammesViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(isWhatsOn, Constants.Messages.ShowAllProgrammesMsg));
                        _navigationService.NavigateTo(Constants.Pages.LiveTv.AllProgrammesView);
                    }
                });
            }
        }

        public RelayCommand PinCommand
        {
            get
            {
                return new RelayCommand(PinTile);
            }
        }

        public RelayCommand<ProgramInfoDto> GuideItemTappedCommand
        {
            get
            {
                return new RelayCommand<ProgramInfoDto>(item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (SimpleIoc.Default.GetInstance<ProgrammeViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ProgrammeItemChangedMsg));
                        _navigationService.NavigateTo(Constants.Pages.LiveTv.ProgrammeView);
                    }
                });
            }
        }

        private void PinTile()
        {
            if (IsPinned)
            {
                var tile = TileService.Current.GetTile(Constants.Pages.LiveTv.LiveTvView);
                if (tile != null)
                {
                    tile.Delete();
                    IsPinned = false;
                }
            }
            else
            {
                var tileData = new ShellTileServiceFlipTileData
                {
                    Title = "MB " + AppResources.LabelLiveTv,
                    BackgroundImage = new Uri("/Assets/Tiles/MBLiveTVTile.png", UriKind.Relative)
                };

                TileService.Current.Create(new Uri(_tileUrl, UriKind.Relative), tileData, false);

                IsPinned = true;
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return;
            }

            await GetWhatsOn(isRefresh);
            await GetUpcoming(isRefresh);
        }

        private async Task GetUpcoming(bool isRefresh)
        {
            if (_upcomingLoaded && !isRefresh && !HasExpired(_upcomingLastRun))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcomingRecordings);

                var query = new RecommendedProgramQuery
                {
                    HasAired = false,
                    Limit = 7,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Upcoming = items.Items.Take(6).ToList();

                    ShowMoreUpcoming = items.Items.Count() > 6;

                    _upcomingLoaded = true;
                    _upcomingLastRun = DateTime.Now;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetWhatsOn(bool isRefresh)
        {
            if (_whatsOnLoaded && !isRefresh && !HasExpired(_whatsOnLastRun))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SystemTrayGettingWhatsOn);

                var query = new RecommendedProgramQuery
                {
                    IsAiring = true,
                    Limit = 7,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetRecommendedLiveTvProgramsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    WhatsOn = items.Items.Take(6).ToList();

                    ShowMoreWhatsOn = items.Items.Count() > 6;

                    _whatsOnLoaded = true;
                    _whatsOnLastRun = DateTime.Now;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetUpcoming()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }

        public bool HasExpired(DateTime? lastRun)
        {
            if (!lastRun.HasValue)
            {
                return true;
            }

            var difference = DateTime.Now - lastRun.Value;
            return difference.TotalMinutes > 30;
        }
    }
}