using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using ScottIsAFool.WindowsPhone;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.DTO;

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
        private readonly ApiClient ApiClient;
        private bool showDataLoaded;
        private bool seasonDataLoaded;
        /// <summary>
        /// Initializes a new instance of the TvViewModel class.
        /// </summary>
        public TvViewModel(INavigationService navService, ApiClient apiClient)
        {
            NavService = navService;
            RecentItems = new ObservableCollection<DTOBaseItem>();
            Episodes = new List<DTOBaseItem>();
            if(IsInDesignMode)
            {
                SelectedTvSeries = new DTOBaseItem
                                       {
                                           Name = "Scrubs"
                                       };
            }
            else
            {
                NavService = navService;
                ApiClient = apiClient;
                WireCommands();
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if(m.Notification.Equals(Constants.ShowTvSeries))
                {
                    showDataLoaded = false;
                    SelectedTvSeries = (DTOBaseItem) m.Sender;
                    DummyFolder = new DTOBaseItem
                    {
                        Type = "folder",
                            Name = SelectedTvSeries.Name + "'s recent items",
                            Id = SelectedTvSeries.Id
                        
                    };
                }
                else if(m.Notification.Equals(Constants.ShowSeasonMsg))
                {
                    seasonDataLoaded = false;
                    SelectedSeason = (DTOBaseItem) m.Sender;
                }
                else if(m.Notification.Equals(Constants.ClearFilmAndTvMsg))
                {
                    SelectedTvSeries = null;
                    SelectedSeason = null;
                    SelectedEpisode = null;
                    Seasons.Clear();
                    Episodes.Clear();
                    RecentItems.Clear();
                    CastAndCrew = null;
                }
                else if(m.Notification.Equals(Constants.ShowEpisodeMsg))
                {
                    SelectedEpisode = (DTOBaseItem) m.Sender;
                }
                else if(m.Notification.Equals(Constants.ClearEpisodesMsg))
                {
                    Episodes.Clear();
                }
            });
        }

        private void WireCommands()
        {
            TvSeriesPageLoaded = new RelayCommand(async () =>
            {
                if(NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !showDataLoaded)
                {
                    if(SelectedTvSeries != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting seasons...";

                        bool seasonsLoaded = await GetSeasons();

                        ProgressText = "Getting recent items...";

                        bool recentItems = await GetRecentItems();

                        ProgressIsVisible = false;
                        ProgressText = "";
                        showDataLoaded = (seasonsLoaded && recentItems);
                    }
                }
            });

            SeasonPageLoaded = new RelayCommand(async () =>
            {
                if(NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !seasonDataLoaded)
                {
                    if(SelectedSeason != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting episodes...";

                        seasonDataLoaded = await GetEpisodes();

                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            EpisodePageLoaded = new RelayCommand(async ()=>
            {
                if(NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort())
                {
                    if(SelectedEpisode != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting episode details...";

                        //bool episodeLoaded = await GetEpisode();

                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            NavigateToPage = new RelayCommand<DTOBaseItem>(NavService.NavigateTopage);
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
                App.ShowMessage("", "Error downloading episode details");
                return false;
            }
        }

        private async Task<bool> GetRecentItems()
        {
            try
            {
                var recent =
                    await ApiClient.GetRecentlyAddedItemsAsync(App.Settings.LoggedInUser.Id, SelectedTvSeries.Id);
                RecentItems.Clear();
                recent.OrderBy(x => x.DateCreated)
                      .Take(6)
                      .ToList()
                      .ForEach(recentItem => RecentItems.Add(recentItem));
                return true;
            }
            catch
            {
                App.ShowMessage("", "Error getting recent items");
                return false;
            }
        }

        private async Task<bool> GetSeasons()
        {
            try
            {
                var seasons = await ApiClient.GetItemAsync(SelectedTvSeries.Id, App.Settings.LoggedInUser.Id);
                Seasons = seasons.Children.ToList();
                CastAndCrew = Utils.GroupCastAndCrew(seasons.People);
                return true;
            }
            catch
            {
                App.ShowMessage("", "Error getting seasons");
                return false;
            }
        }

        private async Task<bool> GetEpisodes()
        {
            try
            {
                var episodes = await ApiClient.GetItemAsync(SelectedSeason.Id, App.Settings.LoggedInUser.Id);
                Episodes = episodes.Children.OrderBy(x => x.IndexNumber).ToList();
                return true;
            }
            catch
            {
                App.ShowMessage("", "Error getting episodes");
                return false;
            }
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public DTOBaseItem SelectedTvSeries { get; set; }
        public List<DTOBaseItem> Seasons { get; set; }
        public List<DTOBaseItem> Episodes { get; set; }
        public DTOBaseItem SelectedEpisode { get; set; }
        public DTOBaseItem SelectedSeason { get; set; }
        public ObservableCollection<DTOBaseItem> RecentItems { get; set; }
        public DTOBaseItem DummyFolder { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand<DTOBaseItem> NavigateToPage { get; set; }
        public RelayCommand TvSeriesPageLoaded { get; set; }
        public RelayCommand SeasonPageLoaded { get; set; }
        public RelayCommand EpisodePageLoaded { get; set; }
    }
}