using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
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
    public class GuideViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _programmesLoaded;

        /// <summary>
        /// Initializes a new instance of the GuideViewModel class.
        /// </summary>
        public GuideViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                SelectedChannel = new ChannelInfoDto
                {
                    Name = "BBC One",
                    Number = "1"
                };

                Programmes = new ObservableCollection<ProgramInfoDto>
                {
                    new ProgramInfoDto
                    {
                        StartDate = new DateTime(2014, 1, 16, 6, 0, 0),
                        Name = "Breakfast News",
                        EpisodeTitle = "16/01/2013",
                        Overview = "The latest news, sport, business and weather from the BBC's Breakfast Team"
                    },
                    new ProgramInfoDto
                    {
                        StartDate = new DateTime(2014, 1, 16, 9, 15, 0),
                        Name = "Wanted Down Under",
                        EpisodeTitle = "Series 8, Davidson Family",
                        Overview = "A mum and son want to move toAustralia, but can they presaude the rest of the family?"
                    }
                };
            }
        }

        public ChannelInfoDto SelectedChannel { get; set; }
        public ObservableCollection<ProgramInfoDto> Programmes { get; set; }

        public ProgramInfoDto SelectedProgramme { get; set; }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }

        public RelayCommand GuidePageLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetProgrammes(false);
                });
            }
        }

        public RelayCommand RefreshGuideCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await GetProgrammes(true);
                });
            }
        }

        public RelayCommand<ProgramInfoDto> GuideItemTappedCommand
        {
            get
            {
                return new RelayCommand<ProgramInfoDto>(item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    if (SimpleIoc.Default.GetInstance<ProgrammeViewModel>() != null)
                    {
                        Messenger.Default.Send(new NotificationMessage(item, Constants.Messages.ProgrammeItemChangedMsg));
                        _navigationService.NavigateTo(Constants.Pages.LiveTv.ProgrammeView);
                    }
                });
            }
        }

        public RelayCommand<ProgramInfoDto> RecordProgrammeCommand
        {
            get
            {
                return new RelayCommand<ProgramInfoDto>(async item =>
                {
                    SetProgressBar(AppResources.SysTraySettingProgrammeToRecord);

                    var id = await LiveTvUtils.RecordProgramme(SelectedProgramme, _apiClient, _navigationService, Log);

                    if (!string.IsNullOrEmpty(id))
                    {
                        item.TimerId = id;
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand<ProgramInfoDto> CreateSeriesLinkCommand
        {
            get
            {
                return new RelayCommand<ProgramInfoDto>(async item =>
                {
                    if (!string.IsNullOrEmpty(item.Id))
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTraySettingSeriesToRecord);

                    var id = await LiveTvUtils.CreateSeriesLink(SelectedProgramme, _apiClient, _navigationService, Log);

                    if (!string.IsNullOrEmpty(id))
                    {
                        item.SeriesTimerId = id;
                    }

                    SetProgressBar();
                });
            }
        } 

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ChangeChannelMsg))
                {
                    _programmesLoaded = false;
                    SelectedChannel = (ChannelInfoDto) m.Sender;
                    Programmes = null;
                }
            });
        }

        private async Task GetProgrammes(bool refresh)
        {
            if (!_navigationService.IsNetworkAvailable || (_programmesLoaded && !refresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingProgrammes);

                var query = new ProgramQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    ChannelIdList = new[] {SelectedChannel.Id},
                    MaxEndDate = DateTime.Now.AddDays(1).Date
                };
                var items = await _apiClient.GetLiveTvProgramsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Programmes = items.Items.ToObservableCollection();
                }

                _programmesLoaded = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetProgrammes()", _navigationService, Log);
            }

            SetProgressBar();
        }
    }
}