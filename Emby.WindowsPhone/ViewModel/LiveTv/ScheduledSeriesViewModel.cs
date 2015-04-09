using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone;

using CustomMessageBox = Emby.WindowsPhone.Controls.CustomMessageBox;

namespace Emby.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScheduledSeriesViewModel : ViewModelBase
    {
        private SeriesTimerInfoDto _originalTimer;
        private bool _scheduledLoaded;
        private bool _recordingsLoaded;

        /// <summary>
        /// Initializes a new instance of the ScheduledSeriesViewModel class.
        /// </summary>
        public ScheduledSeriesViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            var days = Enum.GetValues(typeof (DayOfWeek)).Cast<DayOfWeek>();
            DaysOfWeekList = days.Select(x => new Day {DayOfWeek = x, DisplayName = x.GetLocalisedName()}).ToList();
        }

        public SeriesTimerInfoDto SelectedSeries { get; set; }

        [UsedImplicitly]
        private void OnSelectedSeriesChanged()
        {
            //ServerIdItem = SelectedSeries;
        }

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
                            await ApiClient.CreateLiveTvSeriesTimerAsync(SelectedSeries);
                        }
                        else
                        {
                            await ApiClient.UpdateLiveTvSeriesTimerAsync(SelectedSeries);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "SaveCommand", NavigationService, Log);
                        App.ShowMessage(AppResources.ErrorMakingChanges);
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

                    if (!await LiveTvHelper.CancelRecording(item, NavigationService, ApiClient, Log))
                    {
                        App.ShowMessage(AppResources.ErrorMakingChanges);
                    }

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
                        NavigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledRecordingView);
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

                    if (!await LiveTvHelper.CancelSeries(SelectedSeries, NavigationService, ApiClient, Log, true))
                    {
                        App.ShowMessage(AppResources.ErrorMakingChanges);
                    }

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
                    IsAdd = (bool)m.Target;
                    await CreateSeriesView();
                }

                if (m.Notification.Equals(Constants.Messages.ScheduledSeriesCancelChangesMsg))
                {
                    SelectedSeries = _originalTimer;
                }
            });
        }

        private async Task CreateSeriesView()
        {
            _originalTimer = await SelectedSeries.Clone();
            _scheduledLoaded = false;
            _recordingsLoaded = false;

            foreach (var day in SelectedSeries.Days)
            {
                var dayOfWeek = DaysOfWeekList.FirstOrDefault(x => x.DayOfWeek == day);
                if (dayOfWeek != null)
                {
                    dayOfWeek.IsSelected = true;
                }
            }
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

                var items = await ApiClient.GetLiveTvTimersAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    var upcomingItems = items.Items;
                    var groupedItems = (from u in upcomingItems
                                        group u by u.StartDate.ToLocalTime().Date
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
                Utils.HandleHttpException(ex, "GetScheduledRecordings()", NavigationService, Log);
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

                var items = await ApiClient.GetLiveTvRecordingsAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Recordings = items.Items.ToList();
                    SetProgressBar();
                    return true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetAlreadyRecorded()", NavigationService, Log);
            }

            SetProgressBar();
            return false;
        }
    }
}