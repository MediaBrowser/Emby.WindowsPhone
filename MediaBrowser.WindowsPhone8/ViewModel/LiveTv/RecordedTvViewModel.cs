using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RecordedTvViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _programmesLoaded;

        private DateTime? _programmesLastRun;

        /// <summary>
        /// Initializes a new instance of the RecordedTvViewModel class.
        /// </summary>
        public RecordedTvViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            GroupBy = RecordedGroupBy.RecordedDate;
        }

        public List<RecordingInfoDto> RecordedProgrammes { get; set; }
        public List<Group<RecordingInfoDto>> GroupedRecordedProgrammes { get; set; }

        public bool HasRecordedItems
        {
            get { return !RecordedProgrammes.IsNullOrEmpty(); }
        }

        public RecordedGroupBy GroupBy { get; set; }

        public RelayCommand RecordedTvViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    GroupBy = App.SpecificSettings.DefaultRecordedGroupBy;
                    await LoadProgrammes(false);
                });
            }
        }

        public RelayCommand<RecordingInfoDto> ItemTappedCommand
        {
            get
            {
                return new RelayCommand<RecordingInfoDto>(item =>
                {
                    
                });
            }
        }

        private async Task LoadProgrammes(bool isRefresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_programmesLoaded && !isRefresh && !LiveTvUtils.HasExpired(_programmesLastRun)))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingRecordedItems);

                var query = new RecordingQuery
                {
                    IsInProgress = false,
                    Status = RecordingStatus.Completed,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetLiveTvRecordingsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    RecordedProgrammes = items.Items.OrderBy(x => x.StartDate).ToList();
                    await GroupProgrammes();

                    _programmesLoaded = true;
                    _programmesLastRun = DateTime.Now;
                }

            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadProgrammes(" + isRefresh + ")", _navigationService, Log);
            }

            SetProgressBar();
        }

        

        private async Task GroupProgrammes()
        {
            if (RecordedProgrammes.IsNullOrEmpty())
            {
                return;
            }

            SetProgressBar(AppResources.SysTrayRegrouping);

            await Task.Run(() =>
            {
                var groupedItems = new List<Group<RecordingInfoDto>>();
                switch (GroupBy)
                {
                    case RecordedGroupBy.RecordedDate:
                        groupedItems = (from p in RecordedProgrammes
                                        group p by p.StartDate.ToLocalTime().Date
                                            into grp
                                            orderby grp.Key descending 
                                            select new Group<RecordingInfoDto>(Utils.CoolDateName(grp.Key), grp)).ToList();

                        break;
                    case RecordedGroupBy.ShowName:
                        groupedItems = (from p in RecordedProgrammes
                                        group p by p.Name
                                            into grp
                                            orderby grp.Key
                                            select new Group<RecordingInfoDto>(grp.Key, grp)).ToList();
                        break;
                    case RecordedGroupBy.Channel:
                        groupedItems = (from p in RecordedProgrammes
                                        group p by p.ChannelName
                                            into grp
                                            orderby grp.Key
                                            select new Group<RecordingInfoDto>(grp.Key, grp)).ToList();
                        break;
                }

                Deployment.Current.Dispatcher.BeginInvoke(() => GroupedRecordedProgrammes = groupedItems);
            });
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.Messages.ChangeRecordingGroupingMsg))
                {
                    GroupBy = (RecordedGroupBy) m.Sender;
                    await GroupProgrammes();

                    SetProgressBar();
                }
            });
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => HasRecordedItems);
        }
    }
}