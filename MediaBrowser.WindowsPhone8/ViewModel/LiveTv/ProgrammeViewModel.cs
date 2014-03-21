using System;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ProgrammeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        public ProgrammeViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public ProgramInfoDto SelectedProgramme { get; set; }

        public bool CanRecord
        {
            get { return SelectedProgramme != null && SelectedProgramme.StartDate > DateTime.Now && string.IsNullOrEmpty(SelectedProgramme.TimerId) && !ProgressIsVisible; }
        }

        public bool CanRecordSeries
        {
            get { return SelectedProgramme != null && SelectedProgramme.IsSeries && string.IsNullOrEmpty(SelectedProgramme.SeriesTimerId) && !ProgressIsVisible; }
        }

        public RelayCommand RecordProgrammeCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    SetProgressBar(AppResources.SysTraySettingProgrammeToRecord);

                    var id = await LiveTvUtils.RecordProgramme(SelectedProgramme, _apiClient, _navigationService, Log);

                    if (!string.IsNullOrEmpty(id))
                    {
                        SelectedProgramme.TimerId = id;
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand CreateSeriesLinkCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    SetProgressBar(AppResources.SysTraySettingSeriesToRecord);

                    var id = await LiveTvUtils.CreateSeriesLink(SelectedProgramme, _apiClient, _navigationService, Log);

                    if (!string.IsNullOrEmpty(id))
                    {
                        SelectedProgramme.SeriesTimerId = id;
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand AdvancedSeriesRecordCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    try
                    {
                        SetProgressBar(AppResources.SysTrayPreparing);

                        var series = await _apiClient.GetDefaultLiveTvTimerInfo(SelectedProgramme.Id, default(CancellationToken));

                        if (series != null && SimpleIoc.Default.GetInstance<ScheduledSeriesViewModel>() != null)
                        {
                            Messenger.Default.Send(new NotificationMessage(series, true, Constants.Messages.ScheduledSeriesChangedMsg));
                            _navigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledSeriesView);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "AdvancedSeriesRecordCommand", _navigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ProgrammeItemChangedMsg))
                {
                    SelectedProgramme = (ProgramInfoDto)m.Sender;
                }
            });
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => CanRecord);
            RaisePropertyChanged(() => CanRecordSeries);
        }
    }
}