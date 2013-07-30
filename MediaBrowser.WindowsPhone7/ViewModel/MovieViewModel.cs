using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.Dto;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Resources;
#if !WP8
using ScottIsAFool.WindowsPhone;
#endif
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MovieViewModel : ViewModelBase
    {
        private readonly INavigationService _navService;
        private readonly ExtendedApiClient _apiClient;

        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(INavigationService navService, ExtendedApiClient apiClient)
        {
            _navService = navService;
            _apiClient = apiClient;

            CanUpdateFavourites = true;
            if (IsInDesignMode)
            {
                SelectedMovie = new BaseItemDto
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

                };
            }
            else
            {
                WireCommands();
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {

            });
        }

        private void WireCommands()
        {
            MoviePageLoaded = new RelayCommand(async () =>
            {
                if (SelectedMovie != null && _navService.IsNetworkAvailable)
                {
                    SetProgressBar(AppResources.SysTrayGettingMovieInfo);

                    await GetMovieDetails();

                    if (SelectedMovie.ProviderIds != null)
                    {
                        if (SelectedMovie != null && SelectedMovie.ProviderIds != null && SelectedMovie.ProviderIds.ContainsKey("Imdb"))
                        {
                            ImdbId = SelectedMovie.ProviderIds["Imdb"];
                        }
                    }

                    if (SelectedMovie.RunTimeTicks.HasValue)
                    {
                        RunTime = TimeSpan.FromTicks(SelectedMovie.RunTimeTicks.Value).ToString();
                    }

                    if (SelectedMovie.UserData == null)
                    {
                        SelectedMovie.UserData = new UserItemDataDto();
                    }

                    SetProgressBar();
                }
            });

            AddRemoveFavouriteCommand = new RelayCommand(async () =>
            {
                try
                {
                    CanUpdateFavourites = false;

                    await _apiClient.UpdateFavoriteStatusAsync(SelectedMovie.Id, AuthenticationService.Current.LoggedInUser.Id, !SelectedMovie.UserData.IsFavorite);
                    SelectedMovie.UserData.IsFavorite = !SelectedMovie.UserData.IsFavorite;
                }
                catch (HttpException ex)
                {
                    Log.ErrorException("AddRemoveFavouriteCommand", ex);
                }
                CanUpdateFavourites = true;
            });

            ShowOtherFilmsCommand = new RelayCommand<BaseItemPerson>(person =>
            {
                App.SelectedItem = person;
                _navService.NavigateTo("/Views/FolderView.xaml");
            });

            NavigateTopage = new RelayCommand<BaseItemDto>(_navService.NavigateTo);
        }

        private async Task<bool> GetMovieDetails()
        {
            bool result;

            try
            {
                Log.Info("Getting details for movie [{0}] ({1})", SelectedMovie.Name, SelectedMovie.Id);

                var item = await _apiClient.GetItemAsync(SelectedMovie.Id, AuthenticationService.Current.LoggedInUser.Id);
                SelectedMovie = item;
                CastAndCrew = Utils.GroupCastAndCrew(item.People);
                result = true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetMovieDetails()", ex);

                App.ShowMessage(AppResources.ErrorGettingExtraInfo);
                result = false;
            }

            return result;
        }

        public bool CanUpdateFavourites { get; set; }
        public string FavouriteText { get; set; }
        public Uri FavouriteIcon { get; set; }

        public BaseItemDto SelectedMovie { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }
        public string ImdbId { get; set; }
        public string RunTime { get; set; }

        public RelayCommand<BaseItemDto> NavigateTopage { get; set; }
        public RelayCommand MoviePageLoaded { get; set; }
        public RelayCommand PlayMovieCommand { get; set; }
        public RelayCommand AddRemoveFavouriteCommand { get; set; }
        public RelayCommand<BaseItemPerson> ShowOtherFilmsCommand { get; set; }
    }
}