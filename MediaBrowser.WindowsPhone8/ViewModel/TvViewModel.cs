using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;


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
    public class TvViewModel : ViewModelBase
    {
        public bool ShowDataLoaded;
        public bool SeasonDataLoaded;

        /// <summary>
        /// Initializes a new instance of the TvViewModel class.
        /// </summary>
        public TvViewModel(INavigationService navigationService, IConnectionManager connectionManager) 
            : base(navigationService, connectionManager)
        {
            RecentItems = new ObservableCollection<BaseItemDto>();
            Episodes = new List<BaseItemDto>();
            CanUpdateFavourites = true;
            if (IsInDesignMode)
            {
                SelectedTvSeries = new BaseItemDto
                {
                    Name = "Scrubs"
                };
                SelectedSeason = new BaseItemDto
                {
                    Name = "Season 1"
                };
                Episodes = new[]
                {
                    new BaseItemDto
                    {
                        Id = "e252ea3059d140a0274282bc8cd194cc",
                        Name = "1x01 - Pilot",
                        Overview =
                            "A Kindergarten teacher starts speaking gibberish and passed out in front of her class. What looks like a possible brain tumor does not respond to treatment and provides many more questions than answers for House and his team as they engage in a risky trial-and-error approach to her case. When the young teacher refuses any additional variations of treatment and her life starts slipping away, House must act against his code of conduct and make a personal visit to his patient to convince her to trust him one last time."
                    }
                }.ToList();
                SelectedEpisode = Episodes[0];

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
                if (m.Notification.Equals(Constants.Messages.ClearEpisodesMsg))
                {
                    Episodes.Clear();
                }
            });
        }

        private void WireCommands()
        {
            TvSeriesPageLoaded = new RelayCommand(async () =>
            {
                DummyFolder = new BaseItemDto
                {
                    Type = "folder",
                    Name = string.Format(AppResources.TvShowRecentItemsTitle, SelectedTvSeries.Name.ToLower()),
                    Id = SelectedTvSeries.Id

                };

                if (NavigationService.IsNetworkAvailable && !ShowDataLoaded)
                {
                    if (SelectedTvSeries != null)
                    {
                        SetProgressBar(AppResources.SysTrayGettingShowInformation);

                        try
                        {
                            Log.Info("Getting information for TV Series [{0}] ({1})", SelectedTvSeries.Name, SelectedTvSeries.Id);

                            SelectedTvSeries = await ApiClient.GetItemAsync(SelectedTvSeries.Id, AuthenticationService.Current.LoggedInUserId);
                            CastAndCrew = Utils.GroupCastAndCrew(SelectedTvSeries.People);
                        }
                        catch (HttpException ex)
                        {
                            if (Utils.HandleHttpException("TvSeriesPageLoaded", ex, NavigationService, Log))
                            {
                                SetProgressBar();
                                return;
                            }
                        }

                        bool seasonsLoaded = await GetSeasons();

                        SetProgressBar(AppResources.SysTrayGettingRecentItems);

                        bool recentItems = await GetRecentItems().ConfigureAwait(true);

                        SetProgressBar();
                        ShowDataLoaded = (seasonsLoaded && recentItems);
                    }
                }
            });

            SeasonPageLoaded = new RelayCommand(async () =>
            {
                if (NavigationService.IsNetworkAvailable && !SeasonDataLoaded)
                {
                    if (SelectedSeason != null)
                    {
                        SetProgressBar(AppResources.SysTrayGettingEpisodes);

                        SeasonDataLoaded = await GetEpisodes();

                        SetProgressBar();
                    }
                }
            });

            EpisodePageLoaded = new RelayCommand(async () =>
            {
                
            });

            NextEpisodeCommand = new RelayCommand(() =>
            {
                SelectedEpisode = SelectedEpisode.IndexNumber + 1 > Episodes.Count ? Episodes[0] : Episodes[SelectedEpisode.IndexNumber.Value];
            });

            PreviousEpisodeCommand = new RelayCommand(() =>
            {
                SelectedEpisode = SelectedEpisode.IndexNumber - 1 == 0 ? Episodes[Episodes.Count - 1] : Episodes[SelectedEpisode.IndexNumber.Value - 2];
            });

            AddRemoveFavouriteCommand = new RelayCommand<BaseItemDto>(async item =>
            {
                try
                {
                    SetProgressBar(AppResources.SysTrayAddingToFavourites); 
                    
                    CanUpdateFavourites = false;

                    item.UserData = await ApiClient.UpdateFavoriteStatusAsync(item.Id, AuthenticationService.Current.LoggedInUserId, !item.UserData.IsFavorite);
                }
                catch (HttpException ex)
                {
                    if (Utils.HandleHttpException("AddRemoveFavouriteCommand (TV)", ex, NavigationService, Log))
                    {
                        SetProgressBar();
                        return;
                    }
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

            NavigateTo = new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);
        }

        private async Task<bool> GetEpisode()
        {
            try
            {
                Log.Info("Getting information for episode [{0}] ({1})", SelectedEpisode.Name, SelectedEpisode.Id);

                var episode = await ApiClient.GetItemAsync(SelectedEpisode.Id, AuthenticationService.Current.LoggedInUserId);
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetEpisode()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorEpisodeDetails);
                return false;
            }
        }

        private async Task<bool> GetRecentItems()
        {
            try
            {
                var query = Utils.GetRecentItemsQuery(SelectedTvSeries.Id, new[] {"Season"});

                Log.Info("Getting recent items for TV Show [{0}] ({1})", SelectedTvSeries.Name, SelectedTvSeries.Id);

                var recent = await ApiClient.GetItemsAsync(query);
                if (recent != null && recent.Items != null)
                {
                    RecentItems.Clear();
                    recent.Items
                          .OrderByDescending(x => x.DateCreated)
                          .Take(6)
                          .ToList()
                          .ForEach(recentItem => RecentItems.Add(recentItem));
                }
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetRecentItems()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorRecentItems);
                return false;
            }
        }

        private async Task<bool> GetSeasons()
        {
            try
            {
                var query = new SeasonQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SeriesId = SelectedTvSeries.Id,
                    Fields = new[]
                    {
                        ItemFields.ParentId,
                        ItemFields.MediaSources, 
                    },
                    IsMissing = App.SpecificSettings.ShowMissingEpisodes,
                    IsVirtualUnaired = App.SpecificSettings.ShowUnairedEpisodes
                };

                Log.Info("Getting seasons for TV Show [{0}] ({1})", SelectedTvSeries.Name, SelectedTvSeries.Id);

                var seasons = await ApiClient.GetSeasonsAsync(query);
                Seasons = seasons.Items.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetSeasons()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorSeasons);
                return false;
            }
        }

        private async Task<bool> GetEpisodes()
        {
            try
            {
                var query = new EpisodeQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SeasonId = SelectedSeason.Id,
                    SeriesId = SelectedSeason.SeriesId,
                    Fields = new[]
                    {
                        ItemFields.ParentId,
                        ItemFields.Overview,
                        ItemFields.MediaSources, 
                    },
                    IsMissing = App.SpecificSettings.ShowMissingEpisodes,
                    IsVirtualUnaired = App.SpecificSettings.ShowUnairedEpisodes
                };

                Log.Info("Getting episodes for Season [{0}] ({1}) of TV Show [{2}] ({3})", SelectedSeason.Name, SelectedSeason.Id, SelectedTvSeries.Name, SelectedTvSeries.Id);

                var episodes = await ApiClient.GetEpisodesAsync(query);
                Episodes = episodes.Items.ToList();
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetEpisodes()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorEpisodes);
                return false;
            }
        }

        public bool EpisodeNavigationEnabled
        {
            get { return Episodes.Count > 1; }
        }

        public async Task GetEpisodeDetails()
        {
            if (NavigationService.IsNetworkAvailable)
            {
                var index = SelectedEpisode.IndexNumber;
                if (SelectedEpisode != null && Episodes.IsNullOrEmpty())
                {
                    SetProgressBar(AppResources.SysTrayGettingEpisodeDetails);

                    try
                    {
                        var query = new EpisodeQuery
                        {
                            UserId = AuthenticationService.Current.LoggedInUserId,
                            SeasonId = SelectedEpisode.SeasonId,
                            SeriesId = SelectedEpisode.SeriesId,
                            Fields = new[]
                            {
                                ItemFields.ParentId,
                                ItemFields.Overview,
                                ItemFields.MediaSources, 
                            }
                        };

                        //Log.Info("Getting episodes for Season [{0}] ({1}) of TV Show [{2}] ({3})", SelectedSeason.Name, SelectedSeason.Id, SelectedTvSeries.Name, SelectedTvSeries.Id);

                        var episodes = await ApiClient.GetEpisodesAsync(query);
                        Episodes = episodes.Items.OrderBy(x => x.IndexNumber).ToList();
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("GetEpisodeDetails()", ex, NavigationService, Log);
                    }

                    SetProgressBar();
                }
                
                if (SelectedEpisode != null)
                {
                    SelectedEpisode = Episodes.FirstOrDefault(x => x.IndexNumber == index);
                }
            }
        }

        [UsedImplicitly]
        private void OnSelectedSeasonChanged()
        {
            Episodes = new List<BaseItemDto>();
            SelectedEpisode = null;
        }

        public RelayCommand<BaseItemDto> MarkAsWatchedEpisodeViewCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    await MarkAsWatched(item);
                    RaisePropertyChanged(() => SelectedEpisode.UserData);
                });
            }
        }

        private async Task MarkAsWatched(BaseItemDto item)
        {
            if (!NavigationService.IsNetworkAvailable)
            {
                return;
            }

            try
            {
                item.UserData = item.UserData.Played 
                    ? await ApiClient.MarkUnplayedAsync(item.Id, AuthenticationService.Current.LoggedInUserId) 
                    : await ApiClient.MarkPlayedAsync(item.Id, AuthenticationService.Current.LoggedInUserId, DateTime.Now);
            }
            catch (HttpException ex)
            {
                MessageBox.Show(AppResources.ErrorProblemUpdatingItem, AppResources.ErrorTitle, MessageBoxButton.OK);
                Utils.HandleHttpException("MarkAsWatchedCommand", ex, NavigationService, Log);
            }
        }

        public BaseItemDto SelectedTvSeries { get; set; }
        public List<BaseItemDto> Seasons { get; set; }
        public List<BaseItemDto> Episodes { get; set; }
        public BaseItemDto SelectedEpisode { get; set; }
        public BaseItemDto SelectedSeason { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public BaseItemDto DummyFolder { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand<BaseItemDto> NavigateTo { get; set; }
        public RelayCommand TvSeriesPageLoaded { get; set; }
        public RelayCommand SeasonPageLoaded { get; set; }
        public RelayCommand EpisodePageLoaded { get; set; }
        public RelayCommand NextEpisodeCommand { get; set; }
        public RelayCommand PreviousEpisodeCommand { get; set; }
        public RelayCommand<BaseItemDto> AddRemoveFavouriteCommand { get; set; }
        public RelayCommand<BaseItemPerson> ShowOtherFilmsCommand { get; set; }
        public bool CanUpdateFavourites { get; set; }
    }
}
