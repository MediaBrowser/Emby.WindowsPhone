
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.WindowsPhone.Extensions;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.ViewModel;

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

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ScheduledSeriesChangedMsg))
                {
                    SelectedSeries = (SeriesTimerInfoDto) m.Sender;
                    _originalTimer = await SelectedSeries.Clone();

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
    }
}