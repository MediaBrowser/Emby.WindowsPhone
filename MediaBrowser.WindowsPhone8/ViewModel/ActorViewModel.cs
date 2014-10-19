using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ActorViewModel : ViewModelBase
    {
        private readonly IExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        private bool _dataLoaded;
        
        /// <summary>
        /// Initializes a new instance of the ActorViewModel class.
        /// </summary>
        public ActorViewModel(IExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            if (IsInDesignMode)
            {
                SelectedPerson = new BaseItemPerson {Name = "Jeff Goldblum"};
                var list = new List<BaseItemDto>
                {
                    new BaseItemDto
                    {
                        Id = "6536a66e10417d69105bae71d41a6e6f",
                        Name = "Jurassic Park",
                        SortName = "Jurassic Park",
                        Overview = "Lots of dinosaurs eating people!",
                        People = new[]
                        {
                            new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                            new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                            new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                            new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                        }
                    }
                };

                Films = Utils.GroupItemsByName(list).Result;
            }
        
        }

        public BaseItemDto SelectedActor { get; set; }
        public BaseItemPerson SelectedPerson { get; set; }
        public List<Group<BaseItemDto>> Films { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_navigationService.IsNetworkAvailable || _dataLoaded)
                    {
                        return;
                    }

                    _dataLoaded = await GetActorInformation();
                });
            }
        }

        private async Task<bool> GetActorInformation()
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingDetails);

                var actorResponse = await _apiClient.GetPersonAsync(SelectedPerson.Name, AuthenticationService.Current.LoggedInUser.Id);

                if (actorResponse == null)
                {
                    return false;
                }

                SelectedActor = actorResponse;

                var query = new ItemQuery
                {
                    Person = SelectedPerson.Name,
                    UserId = AuthenticationService.Current.LoggedInUser.Id,
                    SortBy = new []{"SortName"},
                    SortOrder = SortOrder.Ascending,
                    Fields = new[] { ItemFields.People },
                    Recursive = true
                };

                var itemResponse = await _apiClient.GetItemsAsync(query, default(CancellationToken));

                return await SetFilms(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetActorInformation()", ex, _navigationService, Log);
            }

            SetProgressBar();
            return false;
        }

        private async Task<bool> SetFilms(ItemsResult itemResponse)
        {
            Films = await Utils.GroupItemsByName(itemResponse.Items);

            SetProgressBar();

            return true;
        }

        public RelayCommand<BaseItemDto> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(_navigationService.NavigateTo);
            }
        }
    }
}