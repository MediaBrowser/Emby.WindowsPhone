using System;
using Emby.WindowsPhone.Helpers;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Messaging;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;

namespace Emby.WindowsPhone.ViewModel.LiveTv
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

        public BaseItemDto SelectedProgramme { get; set; }

        public BaseItemDto SelectedRecording { get; set; }

        public bool IsRecordNotCancel
        {
            get { return SelectedProgramme != null && string.IsNullOrEmpty(SelectedProgramme.TimerId); }
        }

        public bool RecordIsEnabled
        {
            get { return SelectedProgramme != null && SelectedProgramme.EndDate.HasValue && SelectedProgramme.EndDate.Value.ToLocalTime() > DateTime.Now && !ProgressIsVisible; }
        }

        public bool IsRecordSeriesNotCancel
        {
            get { return SelectedProgramme != null && string.IsNullOrEmpty(SelectedProgramme.SeriesTimerId); }
        }

        public bool SeriesIsEnabled
        {
            get
            {
                return SelectedProgramme != null && SelectedProgramme.IsSeries.HasValue && SelectedProgramme.IsSeries.Value && !ProgressIsVisible;
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

                        var id = await LiveTvHelper.RecordProgramme(SelectedProgramme, ApiClient, NavigationService, Log);

                        if (!string.IsNullOrEmpty(id))
                        {
                            SelectedProgramme.TimerId = id;
                        }
                    }
                    else
                    {
                        SetProgressBar(AppResources.SysTrayCancellingProgramme);

                        if (await LiveTvHelper.CancelRecording(SelectedProgramme.TimerId, NavigationService, ApiClient, Log))
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

                        var id = await LiveTvHelper.CreateSeriesLink(SelectedProgramme, ApiClient, NavigationService, Log);

                        if (!string.IsNullOrEmpty(id))
                        {
                            SelectedProgramme.SeriesTimerId = id;
                        }
                    }
                    else
                    {
                        SetProgressBar(AppResources.SysTrayCancellingSeriesRecording);

                        if (await LiveTvHelper.CancelSeries(SelectedProgramme.SeriesTimerId, NavigationService, ApiClient, Log, false))
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
                        Messenger.Default.Send(new VideoMessage(SelectedRecording, false, PlayerSourceType.Recording));
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
                        Messenger.Default.Send(new VideoMessage(SelectedProgramme, false, PlayerSourceType.Programme));
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
                    var item = m.Sender as BaseItemDto;
                    if (item == null) return;

                    if (item.Type == "Program")
                    {
                        SelectedProgramme = item;
                        //ServerIdItem = SelectedProgramme;
                    }
                    else if (item.Type == "Recording")
                    {
                        SelectedRecording = item;
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