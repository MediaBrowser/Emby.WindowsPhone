using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Controls;
using ScottIsAFool.WindowsPhone;

using CustomMessageBox = MediaBrowser.WindowsPhone.Controls.CustomMessageBox;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ScheduleViewModel : ViewModelBase
    {
        private bool _seriesLoaded;
        private bool _upcomingLoaded;

        private const int UpcomingIndex = 0;
        private const int SeriesIndex = 1;

        /// <summary>
        /// Initializes a new instance of the ScheduleViewModel class.
        /// </summary>
        public ScheduleViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public int SelectedIndex { get; set; }
        public ObservableCollection<SeriesTimerInfoDto> Series { get; set; }
        public List<Group<TimerInfoDto>> Upcoming { get; set; }
        public SeriesTimerInfoDto SelectedSeries { get; set; }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }

        public RelayCommand SchedulePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetData(true);
                });
            }
        }

        public RelayCommand<SeriesTimerInfoDto> CancelSeriesCommand
        {
            get
            {
                return new RelayCommand<SeriesTimerInfoDto>(async series =>
                {
                    SetProgressBar(AppResources.SysTrayCancellingSeriesRecording);

                    await LiveTvUtils.CancelSeries(SelectedSeries, _navigationService, _apiClient, Log, false);

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

        public RelayCommand<SeriesTimerInfoDto> ShowSeriesCommand
        {
            get
            {
                return new RelayCommand<SeriesTimerInfoDto>(series =>
                {
                    if (SimpleIoc.Default.GetInstance<ScheduledSeriesViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(series, false, Constants.Messages.ScheduledSeriesChangedMsg));
                        _navigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledSeriesView);
                    }
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
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ScheduledRecordingChangedMsg));

                    _navigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledRecordingView);
                });
            }
        }

        [UsedImplicitly]
        private async void OnSelectedIndexChanged()
        {
            await GetData(false);
        }

        private async Task GetData(bool isRefresh)
        {
            switch (SelectedIndex)
            {
                case SeriesIndex:
                    await GetScheduledSeriesRecordings(isRefresh);
                    break;
                case UpcomingIndex:
                    await GetScheduledUpcomingRecordings(isRefresh);
                    break;
            }
        }

        private async Task GetScheduledSeriesRecordings(bool refresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_seriesLoaded && !refresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingSeriesRecordings);

                var query = new SeriesTimerQuery();
                var items = await _apiClient.GetLiveTvSeriesTimersAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Series = new ObservableCollection<SeriesTimerInfoDto>();
                    items.Items.ToList().ForEach(Series.Add);
                    _seriesLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetScheduledRecordings()", _navigationService, Log);
            }

            SetProgressBar();
        }

        private async Task GetScheduledUpcomingRecordings(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_upcomingLoaded & !isRefresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingUpcomingRecordings);

                var items = await _apiClient.GetLiveTvTimersAsync(new TimerQuery(), default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    var upcomingItems = items.Items;
                    var groupedItems = (from u in upcomingItems
                                        group u by u.StartDate.ToLocalTime().Date
                                            into grp
                                            orderby grp.Key
                                            select new Group<TimerInfoDto>(Utils.CoolDateName(grp.Key), grp)).ToList();


                    Upcoming = groupedItems;

                    _upcomingLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetScheduledUpcomingRecordings()", ex, _navigationService, Log);
            }

            SetProgressBar();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.LiveTvSeriesDeletedMsg))
                {
                    var seriesId = (string)m.Sender;

                    var seriesToDelete = Series.FirstOrDefault(x => x.Id == seriesId);
                    Series.Remove(seriesToDelete);
                }

                if (m.Notification.Equals(Constants.Messages.NewSeriesRecordingAddedMsg))
                {
                    _seriesLoaded = false;
                }
            });
        }
    }
}