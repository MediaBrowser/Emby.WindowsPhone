using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.DTO;
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
        private readonly INavigationService NavService;
        private readonly ExtendedApiClient ApiClient;
        public bool showDataLoaded;
        public bool seasonDataLoaded;
        /// <summary>
        /// Initializes a new instance of the TvViewModel class.
        /// </summary>
        public TvViewModel(INavigationService navService, ExtendedApiClient apiClient)
        {
            NavService = navService;
            ApiClient = apiClient;
            RecentItems = new ObservableCollection<DtoBaseItem>();
            Episodes = new List<DtoBaseItem>();
            if(IsInDesignMode)
            {
                SelectedTvSeries = new DtoBaseItem
                                       {
                                           Name = "Scrubs"
                                       };
                SelectedSeason = new DtoBaseItem
                                     {
                                         Name = "Season 1"
                                     };
                Episodes = new[]
                               {
                                   new DtoBaseItem
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
                DummyFolder = new DtoBaseItem
                {
                    Type = "folder",
                    Name = string.Format(AppResources.TvShowRecentItemsTitle, SelectedTvSeries.Name.ToLower()),
                    Id = SelectedTvSeries.Id

                };
                if(NavService.IsNetworkAvailable && !showDataLoaded)
                {
                    if(SelectedTvSeries != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = AppResources.SysTrayGettingShowInformation;

                        try
                        {
                            SelectedTvSeries = await ApiClient.GetItemAsync(SelectedTvSeries.Id, App.Settings.LoggedInUser.Id);
                            CastAndCrew = Utils.GroupCastAndCrew(SelectedTvSeries.People);
                        }
                        catch
                        {
                        }

                        bool seasonsLoaded = await GetSeasons();

                        ProgressText = AppResources.SysTrayGettingRecentItems;

                        bool recentItems = await GetRecentItems().ConfigureAwait(true);

                        ProgressIsVisible = false;
                        ProgressText = "";
                        showDataLoaded = (seasonsLoaded && recentItems);
                    }
                }
            });

            SeasonPageLoaded = new RelayCommand(async () =>
            {
                if(NavService.IsNetworkAvailable && !seasonDataLoaded)
                {
                    if(SelectedSeason != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = AppResources.SysTrayGettingEpisodes;

                        seasonDataLoaded = await GetEpisodes();

                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            EpisodePageLoaded = new RelayCommand(async ()=>
            {
                if(NavService.IsNetworkAvailable)
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

            NavigateToPage = new RelayCommand<DtoBaseItem>(NavService.NavigateToPage);
        }

        private async Task<bool> GetEpisode()
        {
            try
            {
                var episode = await ApiClient.GetItemAsync(SelectedEpisode.Id, App.Settings.LoggedInUser.Id);
                return true;
            }
            catch
            {
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
                var recent = await ApiClient.GetItemsAsync(query);
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
            catch
            {
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
                var seasons = await ApiClient.GetItemsAsync(query);
                Seasons = seasons.Items.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch
            {
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
                var episodes = await ApiClient.GetItemsAsync(query);
                Episodes = episodes.Items.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch
            {
                App.ShowMessage("", AppResources.ErrorEpisodes);
                return false;
            }
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public DtoBaseItem SelectedTvSeries { get; set; }
        public List<DtoBaseItem> Seasons { get; set; }
        public List<DtoBaseItem> Episodes { get; set; }
        public DtoBaseItem SelectedEpisode { get; set; }
        public DtoBaseItem SelectedSeason { get; set; }
        public ObservableCollection<DtoBaseItem> RecentItems { get; set; }
        public DtoBaseItem DummyFolder { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand<DtoBaseItem> NavigateToPage { get; set; }
        public RelayCommand TvSeriesPageLoaded { get; set; }
        public RelayCommand SeasonPageLoaded { get; set; }
        public RelayCommand EpisodePageLoaded { get; set; }
        public RelayCommand NextEpisodeCommand { get; set; }
        public RelayCommand PreviousEpisodeCommand { get; set; }
    }
}