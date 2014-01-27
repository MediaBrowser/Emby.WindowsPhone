
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the ScheduledSeriesViewModel class.
        /// </summary>
        public ScheduledSeriesViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            var days = Enum.GetValues(typeof (DayOfWeek)).Cast<DayOfWeek>();
            DaysOfWeekList = days.Select(x => x.GetLocalisedDay()).ToList();
        }

        public SeriesTimerInfoDto SelectedSeries { get; set; }
        public List<string> DaysOfWeekList { get; set; }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ScheduledSeriesChangedMsg))
                {
                    SelectedSeries = (SeriesTimerInfoDto) m.Sender;
                }
            });
        }
    }
}