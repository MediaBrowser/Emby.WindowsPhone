using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone;
using Telerik.Windows.Controls;
using ViewModelBase = ScottIsAFool.WindowsPhone.ViewModel.ViewModelBase;

namespace MediaBrowser.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChannelsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _channelsLoaded;

        /// <summary>
        /// Initializes a new instance of the ChannelsViewModel class.
        /// </summary>
        public ChannelsViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                Channels = new List<ChannelInfoDto>
                {
                    new ChannelInfoDto
                    {
                        Name = "BBC One",
                        Number = "1"
                    },
                    new ChannelInfoDto
                    {
                        Name = "BBC Two",
                        Number = "2"
                    }
                };
                GroupChannels().ConfigureAwait(false);
            }
        }

        public List<ChannelInfoDto> Channels { get; set; }
        public List<Group<ChannelInfoDto>> GroupedChannels { get; set; }

        public RelayCommand<string> NavigateToPage
        {
            get
            {
                return new RelayCommand<string>(_navigationService.NavigateTo);
            }
        }

        public RelayCommand ChannelsViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_channelsLoaded)
                    {
                        _channelsLoaded = await GetChannels();
                    }
                });
            }
        }

        public RelayCommand RefreshChannelsCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    _channelsLoaded = await GetChannels();
                });
            }
        }

        public RelayCommand<ChannelInfoDto> ChannelTappedCommand
        {
            get
            {
                return new RelayCommand<ChannelInfoDto>(channel =>
                {
                    if (SimpleIoc.Default.GetInstance<GuideViewModel>() != null)
                        Messenger.Default.Send(new NotificationMessage(channel, Constants.Messages.ChangeChannelMsg));
                    _navigationService.NavigateTo(Constants.Pages.LiveTv.GuideView);
                });
            }
        }

        private async Task<bool> GetChannels()
        {
            if (!_navigationService.IsNetworkAvailable)
            {
                return false;
            }

            try
            {
                SetProgressBar("Getting channels...");

                var query = new ChannelQuery
                {
                    ChannelType = ChannelType.TV,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };

                var items = await _apiClient.GetLiveTvChannelsAsync(query, default(CancellationToken));

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Channels = items.Items.ToList();
                    await GroupChannels();
                }

                SetProgressBar();
                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetChannels()", ex);
            }

            SetProgressBar();
            return false;
        }

        private async Task GroupChannels()
        {
            var emptyGroups = new List<Group<ChannelInfoDto>>();
            var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            headers.ForEach(item => emptyGroups.Add(new Group<ChannelInfoDto>(item, new List<ChannelInfoDto>())));
            var groupedNameItems = (from c in Channels
                                    group c by Utils.GetSortByNameHeader(c)
                                        into grp
                                        orderby grp.Key
                                        select new Group<BaseItemDto>(grp.Key, grp)).ToList();

            GroupedChannels = groupedNameItems;
        }
    }
}