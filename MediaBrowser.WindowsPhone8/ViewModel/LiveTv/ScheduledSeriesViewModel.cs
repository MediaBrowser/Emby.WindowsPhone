using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;
using CustomMessageBox = MediaBrowser.WindowsPhone.Controls.CustomMessageBox;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScheduledSeriesViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private SeriesTimerInfoDto _originalTimer;
        private bool _scheduledLoaded;
        private bool _recordingsLoaded;

        /// <summary>
        /// Initializes a new instance of the ScheduledSeriesViewModel class.
        /// </summary>
        public ScheduledSeriesViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            var days = Enum.GetValues(typeof (DayOfWeek)).Cast<DayOfWeek>();
            DaysOfWeekList = days.Select(x => new Day {DayOfWeek = x, DisplayName = x.GetLocalisedDay()}).ToList();
        }

        public SeriesTimerInfoDto SelectedSeries { get; set; }
        public List<Day> DaysOfWeekList { get; set; }
        public int SelectedPivotIndex { get; set; }
        public List<Group<TimerInfoDto>> ScheduledRecordings { get; set; }
        public List<RecordingInfoDto> Recordings { get; set; }
        public bool IsAdd { get; set; }

        public int AppBarIndex
        {
            get { return SelectedPivotIndex >= 1 ? 1 : 0; }
        }

        public RelayCommand SaveCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var days = DaysOfWeekList.Where(x => x.IsSelected)
                                             .Select(x => x.DayOfWeek)
                                             .ToList();

                    SelectedSeries.Days = days;

                    SetProgressBar(AppResources.SysTraySaving);

                    try
                    {
                        if (IsAdd)
                        {
                            await _apiClient.CreateLiveTvSeriesTimerAsync(SelectedSeries, default(CancellationToken));
                        }
                        else
                        {
                            await _apiClient.UpdateLiveTvSeriesTimerAsync(SelectedSeries, default(CancellationToken));
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "SaveCommand", _navigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand<TimerInfoDto> CancelRecordingCommand
        {
            get
            {
                return new RelayCommand<TimerInfoDto>(async item =>
                {
                    var messageBox = new CustomMessageBox
                    {
                        Title = AppResources.MessageAreYouSureTitle,
                        Message = AppResources.MessageCancelRecording,
                        LeftButtonContent = AppResources.LabelYes,
                        RightButtonContent = AppResources.LabelNo
                    };

                    var result = await messageBox.ShowAsync();
                    if (result == CustomMessageBoxResult.RightButton)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayCancellingProgramme);

                    await LiveTvUtils.CancelRecording(item, _navigationService, _apiClient, Log);

                    SetProgressBar();
                });
            }
        }

        public RelayCommand<TimerInfoDto> RecordingTappedCommand
        {
            get
            {
                return new RelayCommand<TimerInfoDto>(item =>
                {
                    if (SimpleIoc.Default.GetInstance<ScheduledRecordingViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ScheduledRecordingChangedMsg));
                        _navigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledRecordingView);
                    }
                });
            }
        }

        public RelayCommand DeleteCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var messageBox = new CustomMessageBox
                    {
                        Title = AppResources.MessageAreYouSureTitle,
                        Message = AppResources.MessageDeleteSeriesRecording,
                        LeftButtonContent = AppResources.LabelYes,
                        RightButtonContent = AppResources.LabelNo
                    };

                    var result = await messageBox.ShowAsync();
                    if (result == CustomMessageBoxResult.RightButton)
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayCancellingSeriesRecording);

                    await LiveTvUtils.CancelSeries(SelectedSeries, _navigationService, _apiClient, Log, true);

                    SetProgressBar();
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

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ScheduledSeriesChangedMsg))
                {
                    SelectedSeries = (SeriesTimerInfoDto) m.Sender;
                    _originalTimer = await SelectedSeries.Clone();
                    _scheduledLoaded = false;
                    _recordingsLoaded = false;
                    IsAdd = (bool) m.Target;

                    foreach (var day in SelectedSeries.Days)
                    {
                        var dayOfWeek = DaysOfWeekList.FirstOrDefault(x => x.DayOfWeek == day);
                        if (dayOfWeek != null)
                        {
                            dayOfWeek.IsSelected = true;
                        }
                    }
                }

                if (m.Notification.Equals(Constants.Messages.ScheduledSeriesCancelChangesMsg))
                {
                    SelectedSeries = _originalTimer;
                }
            });
        }

        [UsedImplicitly]
        private async void OnSelectedPivotIndexChanged()
        {
            await LoadData(false);
        }

        private async Task LoadData(bool isRefresh)
        {
            switch (SelectedPivotIndex)
            {
                case 1:
                    if (_recordingsLoaded && !isRefresh)
                    {
                        return;
                    }

                    _recordingsLoaded = await GetAlreadyRecorded();
                    break;
                case 2:
                    if (_scheduledLoaded && !isRefresh)
                    {
                        return;
                    }

                    _scheduledLoaded = await GetScheduledRecordings();
                    break;
            }
        }

        private async Task<bool> GetScheduledRecordings()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcomingRecordings);

                var query = new TimerQuery
                {
                    SeriesTimerId = SelectedSeries.Id
                };

                var items = await _apiClient.GetLiveTvTimersAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    var upcomingItems = items.Items;
                    var groupedItems = (from u in upcomingItems
                                        group u by u.StartDate
                                            into grp
                                            orderby grp.Key
                                            select new Group<TimerInfoDto>(Utils.CoolDateName(grp.Key), grp)).ToList();


                    ScheduledRecordings = groupedItems;
                    
                    SetProgressBar();
                    return true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetScheduledRecordings()", _navigationService, Log);
            }

            SetProgressBar();
            return false;
        }

        private async Task<bool> GetAlreadyRecorded()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingRecordedItems);

                var query = new RecordingQuery
                {
                    SeriesTimerId = SelectedSeries.Id,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetLiveTvRecordingsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Recordings = items.Items.ToList();
                    SetProgressBar();
                    return true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetAlreadyRecorded()", _navigationService, Log);
            }

            SetProgressBar();
            return false;
        }
    }
}