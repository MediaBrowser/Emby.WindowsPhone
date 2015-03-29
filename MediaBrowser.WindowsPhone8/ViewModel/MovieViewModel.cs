using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.CimbalinoToolkit;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

using INavigationService = MediaBrowser.WindowsPhone.Model.Interfaces.INavigationService;
using LockScreenService = MediaBrowser.WindowsPhone.Services.LockScreenService;

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
        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
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
        
        private void WireCommands()
        {
            MoviePageLoaded = new RelayCommand(async () =>
            {
                if (SelectedMovie != null && NavigationService.IsNetworkAvailable)
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
                        var ts = TimeSpan.FromTicks(SelectedMovie.RunTimeTicks.Value);
                        var runtime = ts.Hours == 0 ? string.Format("{0}m", ts.Minutes) : string.Format("{0}h {1}m", ts.Hours, ts.Minutes);
                        RunTime = runtime;
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
                    SetProgressBar(AppResources.SysTrayAddingToFavourites);

                    CanUpdateFavourites = false;

                    SelectedMovie.UserData = await ApiClient.UpdateFavoriteStatusAsync(SelectedMovie.Id, AuthenticationService.Current.LoggedInUserId, !SelectedMovie.UserData.IsFavorite);
                }
                catch (HttpException ex)
                {
                    Utils.HandleHttpException("AddRemoveFavouriteCommand (Movies)", ex, NavigationService, Log);
                    App.ShowMessage(AppResources.ErrorMakingChanges);
                }

                SetProgressBar();

                CanUpdateFavourites = true;
            });

            ShowOtherFilmsCommand = new RelayCommand<BaseItemPerson>(person =>
            {
                App.SelectedItem = person;
                NavigationService.NavigateTo(Constants.Pages.ActorView);
            });

            NavigateTopage = new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);

            SetPosterAsLockScreenCommand = new RelayCommand(async () =>
            {
                if (!LockScreenService.Current.IsProvidedByCurrentApplication)
                {
                    var result = await LockScreenService.Current.RequestAccessAsync();

                    if (result == LockScreenServiceRequestResult.Denied)
                    {
                        return;
                    }
                }

                var url = ApiClient.GetImageUrl(SelectedMovie, LockScreenService.Current.SinglePosterOptions);

                LockScreenService.Current.ManuallySet = true;
                await LockScreenService.Current.SetLockScreenImage(url);
            });
        }

        private async Task<bool> GetMovieDetails()
        {
            bool result;

            try
            {
                Log.Info("Getting details for movie [{0}] ({1})", SelectedMovie.Name, SelectedMovie.Id);

                var item = await ApiClient.GetItemAsync(SelectedMovie.Id, AuthenticationService.Current.LoggedInUserId);
                SelectedMovie = item;
                CastAndCrew = Utils.GroupCastAndCrew(item.People);
                Chapters = SelectedMovie.Chapters.Select(x => new Chapter(x)
                {
                    ImageUrl = GetChapterUrl(x)
                }).ToList();

                if (SelectedMovie.LocalTrailerCount.HasValue && SelectedMovie.LocalTrailerCount.Value > 0)
                {
                    var trailers = await ApiClient.GetLocalTrailersAsync(AuthenticationService.Current.LoggedInUserId, SelectedMovie.Id);
                    Trailers = trailers.ToList();
                }

                CanResume = SelectedMovie.CanResume;

                result = true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetMovieDetails()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorGettingExtraInfo);
                result = false;
            }

            return result;
        }

        private string GetChapterUrl(ChapterInfoDto chapter)
        {
            var imageOptions = new ImageOptions
            {
                MaxHeight = 173,
                ImageIndex = SelectedMovie.Chapters.IndexOf(chapter),
                ImageType = ImageType.Chapter,
                Tag = chapter.ImageTag,
                EnableImageEnhancers = App.SpecificSettings.EnableImageEnhancers
            };

            return chapter.HasImage ? ApiClient.GetImageUrl(SelectedMovie, imageOptions) : string.Empty;
        }

        public bool CanUpdateFavourites { get; set; }
        public string FavouriteText { get; set; }
        public Uri FavouriteIcon { get; set; }

        public BaseItemDto SelectedMovie { get; set; }

        [UsedImplicitly]
        private void OnSelectedMovieChanged()
        {
            ServerIdItem = SelectedMovie;
            CanResume = SelectedMovie != null && SelectedMovie.CanResume;
        }

        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }
        public string ImdbId { get; set; }
        public string RunTime { get; set; }
        public List<Chapter> Chapters { get; set; }
        public List<BaseItemDto> Trailers { get; set; }
        public bool CanResume { get; set; }

        public RelayCommand<BaseItemDto> NavigateTopage { get; set; }
        public RelayCommand MoviePageLoaded { get; set; }
        public RelayCommand PlayMovieCommand { get; set; }
        public RelayCommand AddRemoveFavouriteCommand { get; set; }
        public RelayCommand<BaseItemPerson> ShowOtherFilmsCommand { get; set; }
        public RelayCommand SetPosterAsLockScreenCommand { get; set; }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.RefreshResumeMsg))
                {
                    var id = (string) m.Sender;
                    var ticks = (long) m.Target;
                    if (id == SelectedMovie.Id)
                    {
                        if (SelectedMovie.UserData == null)
                        {
                            SelectedMovie.UserData = new UserItemDataDto();
                        }

                        SelectedMovie.UserData.PlaybackPositionTicks = ticks;

                        CanResume = SelectedMovie.CanResume;
                    }
                }
            });
        }
    }
}