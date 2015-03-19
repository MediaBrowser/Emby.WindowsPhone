using System;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;


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
        public ProgrammeViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public ProgramInfoDto SelectedProgramme { get; set; }

        public RecordingInfoDto SelectedRecording { get; set; }

        public bool IsRecordNotCancel
        {
            get { return SelectedProgramme != null && string.IsNullOrEmpty(SelectedProgramme.TimerId); }
        }

        public bool RecordIsEnabled
        {
            get { return SelectedProgramme != null && SelectedProgramme.EndDate.ToLocalTime() > DateTime.Now && !ProgressIsVisible; }
        }

        public bool IsRecordSeriesNotCancel
        {
            get { return SelectedProgramme != null && string.IsNullOrEmpty(SelectedProgramme.SeriesTimerId); }
        }

        public bool SeriesIsEnabled
        {
            get
            {
                return SelectedProgramme != null && SelectedProgramme.IsSeries && !ProgressIsVisible;
            }
        }

        public RelayCommand RecordProgrammeCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (IsRecordNotCancel)
                    {
                        SetProgressBar(AppResources.SysTraySettingProgrammeToRecord);

                        var id = await LiveTvUtils.RecordProgramme(SelectedProgramme, ApiClient, NavigationService, Log);

                        if (!string.IsNullOrEmpty(id))
                        {
                            SelectedProgramme.TimerId = id;
                        }
                    }
                    else
                    {
                        SetProgressBar(AppResources.SysTrayCancellingProgramme);

                        if (await LiveTvUtils.CancelRecording(SelectedProgramme.TimerId, NavigationService, ApiClient, Log))
                        {
                            SelectedProgramme.TimerId = null;
                        }
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
                    if (IsRecordSeriesNotCancel)
                    {
                        SetProgressBar(AppResources.SysTraySettingSeriesToRecord);

                        var id = await LiveTvUtils.CreateSeriesLink(SelectedProgramme, ApiClient, NavigationService, Log);

                        if (!string.IsNullOrEmpty(id))
                        {
                            SelectedProgramme.SeriesTimerId = id;
                        }
                    }
                    else
                    {
                        SetProgressBar(AppResources.SysTrayCancellingSeriesRecording);

                        if (await LiveTvUtils.CancelSeries(SelectedProgramme.SeriesTimerId, NavigationService, ApiClient, Log, false))
                        {
                            SelectedProgramme.SeriesTimerId = null;
                        }
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand ShowSeriesRecordingCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (SelectedProgramme == null || string.IsNullOrEmpty(SelectedProgramme.SeriesTimerId))
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayGettingRecordingDetails);

                    try
                    {
                        var series = await ApiClient.GetLiveTvSeriesTimerAsync(SelectedProgramme.SeriesTimerId);
                        if (SimpleIoc.Default.GetInstance<ScheduledSeriesViewModel>() != null)
                        {
                            Messenger.Default.Send(new NotificationMessage(series, false, Constants.Messages.ScheduledSeriesChangedMsg));
                            NavigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledSeriesView);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "ShowSeriesRecordingCommand", NavigationService, Log);
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

                        var series = await ApiClient.GetDefaultLiveTvTimerInfo(SelectedProgramme.Id);

                        if (series != null && SimpleIoc.Default.GetInstance<ScheduledSeriesViewModel>() != null)
                        {
                            Messenger.Default.Send(new NotificationMessage(series, true, Constants.Messages.ScheduledSeriesChangedMsg));
                            NavigationService.NavigateTo(Constants.Pages.LiveTv.ScheduledSeriesView);
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "AdvancedSeriesRecordCommand", NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand PlayRecordingCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null)
                    {
                        Messenger.Default.Send(new VideoMessage(SelectedRecording, false));
                        NavigationService.NavigateTo(string.Format(Constants.Pages.VideoPlayerView, SelectedRecording.Id, SelectedRecording.Type));
                    }
                });
            }
        }

        public RelayCommand PlayProgrammeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null)
                    {
                        Messenger.Default.Send(new VideoMessage(SelectedProgramme, false));
                        NavigationService.NavigateTo(string.Format(Constants.Pages.VideoPlayerView,  SelectedProgramme.Id, SelectedProgramme.Type));
                    }
                });
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ProgrammeItemChangedMsg))
                {
                    if (m.Sender is ProgramInfoDto)
                    {
                        SelectedProgramme = (ProgramInfoDto)m.Sender;
                        //ServerIdItem = SelectedProgramme;
                    }
                    else if (m.Sender is RecordingInfoDto)
                    {
                        SelectedRecording = (RecordingInfoDto) m.Sender;
                        //ServerIdItem = SelectedRecording;
                    }
                }
            });
        }

        public override void UpdateProperties()
        {
            RaisePropertyChanged(() => RecordIsEnabled);
            RaisePropertyChanged(() => SeriesIsEnabled);
            RaisePropertyChanged(() => IsRecordNotCancel);
            RaisePropertyChanged(() => IsRecordSeriesNotCancel);
            RaisePropertyChanged(() => SelectedProgramme);
        }
    }
}