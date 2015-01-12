using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

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
        private bool _dataLoaded;
        
        /// <summary>
        /// Initializes a new instance of the ActorViewModel class.
        /// </summary>
        public ActorViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base (navigationService, connectionManager)
        {
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

        [UsedImplicitly]
        private void OnSelectedPersonChanged()
        {
            //ServerIdItem = SelectedPerson;
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!NavigationService.IsNetworkAvailable || _dataLoaded)
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

                var actorResponse = await ApiClient.GetItemAsync(SelectedPerson.Id, AuthenticationService.Current.LoggedInUserId);

                if (actorResponse == null)
                {
                    return false;
                }

                SelectedActor = actorResponse;
                ServerIdItem = SelectedActor;

                var query = new ItemQuery
                {
                    Person = SelectedPerson.Name,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new []{"SortName"},
                    SortOrder = SortOrder.Ascending,
                    Fields = new[] { ItemFields.People },
                    Recursive = true
                };

                var itemResponse = await ApiClient.GetItemsAsync(query);

                return await SetFilms(itemResponse);
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetActorInformation()", ex, NavigationService, Log);
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
                return new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);
            }
        }
    }
}