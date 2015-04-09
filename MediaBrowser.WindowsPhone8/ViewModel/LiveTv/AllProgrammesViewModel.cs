using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using INavigationService = Emby.WindowsPhone.Model.Interfaces.INavigationService;

namespace Emby.WindowsPhone.ViewModel.LiveTv
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class AllProgrammesViewModel : ViewModelBase
    {
        private bool _programmesLoaded;
        private bool _isWhatsOn;

        /// <summary>
        /// Initializes a new instance of the AllProgrammesViewModel class.
        /// </summary>
        public AllProgrammesViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
        }

        public List<ProgramInfoDto> Programmes { get; set; }
        public string PageTitle { get; set; }

        public RelayCommand AllProgrammesViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (!NavigationService.IsNetworkAvailable || (_programmesLoaded && !isRefresh))
            {
                return;
            }

            RecommendedProgramQuery query;

            if (_isWhatsOn)
            {
                query = new RecommendedProgramQuery
                {
                    IsAiring = true,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };
            }
            else
            {
                query = new RecommendedProgramQuery
                {
                    HasAired = false,
                    UserId = AuthenticationService.Current.LoggedInUserId
                };
            }

            SetProgressBar(AppResources.SysTrayLoadingProgrammes);

            try
            {
                var items = await ApiClient.GetRecommendedLiveTvProgramsAsync(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    Programmes = items.Items.ToList();
                    _programmesLoaded = true;
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", NavigationService, Log);
            }

            SetProgressBar();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShowAllProgrammesMsg))
                {
                    _isWhatsOn = (bool) m.Sender;
                    _programmesLoaded = false;
                    Programmes = null;

                    PageTitle = _isWhatsOn ? AppResources.LabelWhatsOn.ToLower() : AppResources.LabelUpcoming.ToLower();
                }
            });
        }
    }
}