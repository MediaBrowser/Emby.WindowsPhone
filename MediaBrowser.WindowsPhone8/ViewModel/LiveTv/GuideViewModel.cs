using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
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

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
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
                }
            });
        }

        private async Task<bool> GetProgrammes()
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return false;
            }

            try
            {
                //var items = await _apiClient.

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetProgrammes()", ex);
            }

            return false;
        }
    }
}