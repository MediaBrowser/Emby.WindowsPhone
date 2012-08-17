using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using ScottIsAFool.WindowsPhone;
using SharpGIS;
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
        private bool showDataLoaded;
        private bool seasonDataLoaded;
        /// <summary>
        /// Initializes a new instance of the TvViewModel class.
        /// </summary>
        public TvViewModel(INavigationService navService)
        {
            NavService = navService;
            RecentItems = new ObservableCollection<BaseItemContainer<ApiBaseItem>>();
            Episodes = new List<BaseItemContainer<ApiBaseItem>>();
            if(IsInDesignMode)
            {
                SelectedTvSeries = new BaseItemContainer<ApiBaseItem>
                                       {
                                           Item = new ApiBaseItem
                                                      {
                                                          Name = "Scrubs"
                                                      }
                                       };
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
                if(m.Notification.Equals(Constants.ShowTvSeries))
                {
                    showDataLoaded = false;
                    SelectedTvSeries = (BaseItemContainer<ApiBaseItem>) m.Sender;
                    DummyFolder = new BaseItemContainer<ApiBaseItem>
                    {
                        Type = "folder",
                        Item = new ApiBaseItem
                        {
                            Name = SelectedTvSeries.Item.Name + "'s recent items",
                            Id = SelectedTvSeries.Item.Id
                        }
                    };
                }
                else if(m.Notification.Equals(Constants.ShowSeasonMsg))
                {
                    seasonDataLoaded = false;
                    SelectedSeason = (BaseItemContainer<ApiBaseItem>) m.Sender;
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


                        ProgressText = string.Empty;
                        ProgressIsVisible = false;
                    }
                }
            });

            NavigateToPage = new RelayCommand<BaseItemContainer<ApiBaseItem>>(NavService.NavigateTopage);
        }

        private async Task<bool> GetRecentItems()
        {
            bool result = false;
            var url = string.Format(App.Settings.ApiUrl + "itemlist?listtype=recentlyaddeditems&userid={0}&id={1}",
                                    App.Settings.LoggedInUser.Id, SelectedTvSeries.Item.Id);

            string recentJson = string.Empty;
            try
            {
                recentJson = await new GZipWebClient().DownloadStringTaskAsync(url);
            }
            catch
            {
                App.ShowMessage("", "Error downloading recent items");
            }

            if(!string.IsNullOrEmpty(recentJson))
            {
                var recent = JsonConvert.DeserializeObject<List<BaseItemContainer<ApiBaseItem>>>(recentJson);
                RecentItems.Clear();
                recent.OrderBy(x => x.Item.DateCreated)
                      .Take(6)
                      .ToList()
                      .ForEach(recentItem => RecentItems.Add(recentItem));
                result = true;
            }

            return result;
        }

        private async Task<bool> GetSeasons()
        {
            bool result = false;
            var url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}",
                                                   App.Settings.LoggedInUser.Id, SelectedTvSeries.Item.Id);

            string seasonJson = string.Empty;
            try
            {
                seasonJson = await new GZipWebClient().DownloadStringTaskAsync(url);
            }
            catch
            {
                App.ShowMessage("", "Error downloading season details");
            }
            if(!string.IsNullOrEmpty(seasonJson))
            {
                var seasons = JsonConvert.DeserializeObject<BaseItemContainer<ApiBaseItem>>(seasonJson);
                Seasons = seasons.Children.ToList();
                CastAndCrew = Utils.GroupCastAndCrew(seasons.People);
                result = true;
            }
            return result;
        }

        private async Task<bool> GetEpisodes()
        {
            bool result = false;

            var url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}",
                                                   App.Settings.LoggedInUser.Id, SelectedSeason.Item.Id);

            string episodeJson = string.Empty;
            try
            {
                episodeJson = await new GZipWebClient().DownloadStringTaskAsync(url);
            }
            catch
            {
                App.ShowMessage("", "Error downloading episodes");
            }
            if(!string.IsNullOrEmpty(episodeJson))
            {
                var episodes = JsonConvert.DeserializeObject<BaseItemContainer<ApiBaseItem>>(episodeJson);
                Episodes = episodes.Children.OrderBy(x => x.Item.IndexNumber).ToList();
                                 
                result = true;
            }

            return result;
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public BaseItemContainer<ApiBaseItem> SelectedTvSeries { get; set; }
        public List<BaseItemContainer<ApiBaseItem>> Seasons { get; set; }
        public List<BaseItemContainer<ApiBaseItem>> Episodes { get; set; }
        public BaseItemContainer<ApiBaseItem> SelectedEpisode { get; set; }
        public BaseItemContainer<ApiBaseItem> SelectedSeason { get; set; }
        public ObservableCollection<BaseItemContainer<ApiBaseItem>> RecentItems { get; set; }
        public BaseItemContainer<ApiBaseItem> DummyFolder { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand<BaseItemContainer<ApiBaseItem>> NavigateToPage { get; set; }
        public RelayCommand TvSeriesPageLoaded { get; set; }
        public RelayCommand SeasonPageLoaded { get; set; }
        public RelayCommand EpisodePageLoaded { get; set; }
    }
}