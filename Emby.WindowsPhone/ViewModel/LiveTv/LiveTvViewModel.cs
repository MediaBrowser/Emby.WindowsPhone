using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using Emby.WindowsPhone.CimbalinoToolkit.Tiles;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using MediaBrowser.Model.Dto;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class LiveTvViewModel : ViewModelBase
    {
        private bool _upcomingLoaded;
        private bool _whatsOnLoaded;
        private bool _currentlyRecordingLoaded;

        private DateTime? _upcomingLastRun;
        private DateTime? _whatsOnLastRun;
        private DateTime? _currentlyRecordingLastRun;

        private readonly string _tileUrl = string.Format(Constants.PhoneTileUrlFormat, "LiveTV", string.Empty, "Live TV");

        /// <summary>
        /// Initializes a new instance of the LiveTvViewModel class.
        /// </summary>
        public LiveTvViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base (navigationService, connectionManager)
        {
        }

        public List<BaseItemDto> WhatsOn { get; set; }
        public List<BaseItemDto> Upcoming { get; set; }
        public List<BaseItemDto> CurrentlyRecording { get; set; }

        public bool ShowMoreWhatsOn { get; set; }
        public bool ShowMoreUpcoming { get; set; }
        public bool IsPinned { get; set; }

        public bool HasWhatsOnItems
        {
            get { return !WhatsOn.IsNullOrEmpty(); }
        }

        public bool HasUpcomingItems
        {
            get { return !Upcoming.IsNullOrEmpty(); }
        }

        public bool HasCurrentlyRecordingItems
        {
            get { return !CurrentlyRecording.IsNullOrEmpty(); }
        }

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

        public RelayCommand<string> MoreCommand
        {
            get
            {
                return new RelayCommand<string>(parameter =>
                {
                    var isWhatsOn = bool.Parse(parameter);
                    if (SimpleIoc.Default.GetInstance<AllProgrammesViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(isWhatsOn, Constants.Messages.ShowAllProgrammesMsg));
                        NavigationService.NavigateTo(Constants.Pages.LiveTv.AllProgrammesView);
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

        public RelayCommand<BaseItemDto> GuideItemTappedCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (SimpleIoc.Default.GetInstance<ProgrammeViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ProgrammeItemChangedMsg));
                        NavigationService.NavigateTo(Constants.Pages.LiveTv.ProgrammeView);
                    }
                });
            }
        }

        public RelayCommand<BaseItemDto> RecordingItemTappedCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (SimpleIoc.Default.GetInstance<ProgrammeViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ProgrammeItemChangedMsg));
                        NavigationService.NavigateTo(Constants.Pages.LiveTv.RecordingView);
                    }
                });
            }
        }

        private void PinTile()
        {
            if (IsPinned)
            {
                var tile = TileService.Current.GetTile(_tileUrl);
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
                    Title = "Emby " + AppResources.LabelLiveTv,
                    BackgroundImage = App.SpecificSettings.UseTransparentTile? new Uri("/Assets/Tiles/MBLiveTVTileTransparent.png", UriKind.Relative) : new Uri("/Assets/Tiles/MBLiveTVTile.png", UriKind.Relative)
                };

                TileService.Current.Create(new Uri(_tileUrl, UriKind.Relative), tileData, false);

                IsPinned = true;
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable)
            {
                return;
            }

            await GetWhatsOn(isRefresh);
            await GetUpcoming(isRefresh);
            await LoadCurrentlyRecording(isRefresh);
        }

        private async Task GetUpcoming(bool isRefresh)
        {
            if (_upcomingLoaded && !isRefresh && !LiveTvHelper.HasExpired(_upcomingLastRun))
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

                var items = await ApiClient.GetRecommendedLiveTvProgramsAsync(query);

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
                Utils.HandleHttpException("GetUpcoming()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetWhatsOn(bool isRefresh)
        {
            if (_whatsOnLoaded && !isRefresh && !LiveTvHelper.HasExpired(_whatsOnLastRun))
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

                var items = await ApiClient.GetRecommendedLiveTvProgramsAsync(query);

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
                Utils.HandleHttpException("GetUpcoming()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        private async Task LoadCurrentlyRecording(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable || (_currentlyRecordingLoaded && !isRefresh && !LiveTvHelper.HasExpired(_currentlyRecordingLastRun, 5)))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingCurrentlyRecording);

                var query = new RecordingQuery
                {
                    IsInProgress = true,
                    Status = RecordingStatus.InProgress,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await ApiClient.GetLiveTvRecordingsAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    CurrentlyRecording = items.Items.ToList();

                    _currentlyRecordingLoaded = true;
                    _currentlyRecordingLastRun = DateTime.Now;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadCurrentlyRecording(" + isRefresh + ")", NavigationService, Log);
            }

            SetProgressBar();
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => HasCurrentlyRecordingItems);
            RaisePropertyChanged(() => HasUpcomingItems);
            RaisePropertyChanged(() => HasWhatsOnItems);
        }
    }
}