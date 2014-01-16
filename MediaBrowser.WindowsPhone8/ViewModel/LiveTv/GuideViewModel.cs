using System.Collections.Generic;
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
        }

        public ChannelInfoDto SelectedChannel { get; set; }

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
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetProgrammes()", ex);
            }
        }
    }
}