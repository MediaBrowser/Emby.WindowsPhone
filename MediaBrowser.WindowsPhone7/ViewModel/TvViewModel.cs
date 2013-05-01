using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Dto;
using MediaBrowser.WindowsPhone.Resources;

#if !WP8
using ScottIsAFool.WindowsPhone;
#endif

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
        private readonly INavigationService _navService;
        private readonly ExtendedApiClient _apiClient;
        private readonly ILog _logger;

        public bool ShowDataLoaded;
        public bool SeasonDataLoaded;
        /// <summary>
        /// Initializes a new instance of the TvViewModel class.
        /// </summary>
        public TvViewModel(INavigationService navService, ExtendedApiClient apiClient)
        {
            _navService = navService;
            _apiClient = apiClient;
            _logger =new WPLogger(typeof(TvViewModel)); 

            RecentItems = new ObservableCollection<BaseItemDto>();
            Episodes = new List<BaseItemDto>();
            if(IsInDesignMode)
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
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if(m.Notification.Equals(Constants.ClearEpisodesMsg))
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
                if(_navService.IsNetworkAvailable && !ShowDataLoaded)
                {
                    if(SelectedTvSeries != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = AppResources.SysTrayGettingShowInformation;

                        try
                        {
                            _logger.LogFormat("Getting information for TV Series [{0}] ({1})", LogLevel.Info, SelectedTvSeries.Name, SelectedTvSeries.Id);

                            SelectedTvSeries = await _apiClient.GetItemAsync(SelectedTvSeries.Id, App.Settings.LoggedInUser.Id);
                            CastAndCrew = Utils.GroupCastAndCrew(SelectedTvSeries.People);
                        }
                        catch (HttpException ex)
                        {
                            _logger.Log(ex.Message, LogLevel.Fatal);
                            _logger.Log(ex.StackTrace, LogLevel.Fatal);
                        }

                        bool seasonsLoaded = await GetSeasons();

                        ProgressText = AppResources.SysTrayGettingRecentItems;

                        bool recentItems = await GetRecentItems().ConfigureAwait(true);

                        ProgressIsVisible = false;
                        ProgressText = "";
                        ShowDataLoaded = (seasonsLoaded && recentItems);
                    }
                }
            });

            SeasonPageLoaded = new RelayCommand(async () =>
            {
                if(_navService.IsNetworkAvailable && !SeasonDataLoaded)
                {
                    if(SelectedSeason != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = AppResources.SysTrayGettingEpisodes;

                        SeasonDataLoaded = await GetEpisodes();

                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            EpisodePageLoaded = new RelayCommand(async ()=>
            {
                if(_navService.IsNetworkAvailable)
                {
                    if(SelectedEpisode != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = AppResources.SysTrayGettingEpisodeDetails;

                        //bool episodeLoaded = await GetEpisode();

                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            NextEpisodeCommand = new RelayCommand(() =>
                                                      {
                                                          SelectedEpisode = SelectedEpisode.IndexNumber + 1 > Episodes.Count ? Episodes[0] : Episodes[SelectedEpisode.IndexNumber.Value];
                                                      });
            PreviousEpisodeCommand = new RelayCommand(()=>
                                                          {
                                                              SelectedEpisode = SelectedEpisode.IndexNumber - 1 == 0 ? Episodes[Episodes.Count - 1] : Episodes[SelectedEpisode.IndexNumber.Value - 2];
                                                          });

            NavigateTo = new RelayCommand<BaseItemDto>(_navService.NavigateTo);
        }

        private async Task<bool> GetEpisode()
        {
            try
            {
                _logger.LogFormat("Getting information for episode [{0}] ({1})", LogLevel.Info, SelectedEpisode.Name, SelectedEpisode.Id);

                var episode = await _apiClient.GetItemAsync(SelectedEpisode.Id, App.Settings.LoggedInUser.Id);
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Log(ex.Message, LogLevel.Fatal);
                _logger.Log(ex.StackTrace, LogLevel.Fatal);

                App.ShowMessage("", AppResources.ErrorEpisodeDetails);
                return false;
            }
        }

        private async Task<bool> GetRecentItems()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    ParentId = SelectedTvSeries.Id,
                    Filters = new[] { ItemFilter.IsRecentlyAdded },
                    Fields = new[]
                                                 {
                                                     ItemFields.SeriesInfo,
                                                     ItemFields.ParentId
                                                 },
                    Recursive = true
                };

                _logger.LogFormat("Getting recent items for TV Show [{0}] ({1})", LogLevel.Info, SelectedTvSeries.Name, SelectedTvSeries.Id);

                var recent = await _apiClient.GetItemsAsync(query);
                if (recent != null && recent.Items != null)
                {
                    RecentItems.Clear();
                    recent.Items.OrderByDescending(x => x.DateCreated)
                          .Take(6)
                          .ToList()
                          .ForEach(recentItem => RecentItems.Add(recentItem));
                }
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Log(ex.Message, LogLevel.Fatal);
                _logger.Log(ex.StackTrace, LogLevel.Fatal);

                App.ShowMessage("", AppResources.ErrorRecentItems);
                return false;
            }
        }

        private async Task<bool> GetSeasons()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    ParentId = SelectedTvSeries.Id,
                    Fields = new[]
                                                 {
                                                     ItemFields.SeriesInfo,
                                                     ItemFields.ParentId
                                                 }
                };

                _logger.LogFormat("Getting seasons for TV Show [{0}] ({1})", LogLevel.Info, SelectedTvSeries.Name, SelectedTvSeries.Id);

                var seasons = await _apiClient.GetItemsAsync(query);
                Seasons = seasons.Items.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Log(ex.Message, LogLevel.Fatal);
                _logger.Log(ex.StackTrace, LogLevel.Fatal);

                App.ShowMessage("", AppResources.ErrorSeasons);
                return false;
            }
        }

        private async Task<bool> GetEpisodes()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    ParentId = SelectedSeason.Id,
                    Fields = new[]
                                                 {
                                                     ItemFields.SeriesInfo,
                                                     ItemFields.ParentId,
                                                     ItemFields.Overview, 
                                                 }
                };

                _logger.LogFormat("Getting episodes for Season [{0}] ({1}) of TV Show [{2}] ({3})", LogLevel.Info, SelectedSeason.Name, SelectedSeason.Id, SelectedTvSeries.Name, SelectedTvSeries.Id);

                var episodes = await _apiClient.GetItemsAsync(query);
                Episodes = episodes.Items.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch (HttpException ex)
            {
                _logger.Log(ex.Message, LogLevel.Fatal);
                _logger.Log(ex.StackTrace, LogLevel.Fatal);

                App.ShowMessage("", AppResources.ErrorEpisodes);
                return false;
            }
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
        public bool EpisodeNavigationEnabled { get { return Episodes.Count > 1; } }

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
    }
}