using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
using SharpGIS;
using System.Linq;

namespace MediaBrowser.ViewModel
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
            if(IsInDesignMode)
            {
                
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
                    SelectedTvSeries = (ApiBaseItemWrapper<ApiBaseItem>) m.Sender;
                }
                else if(m.Notification.Equals(Constants.ClearFilmAndTvMsg))
                {
                    SelectedTvSeries = null;
                    SelectedSeason = null;
                    SelectedEpisode = null;
                    Seasons = null;
                    Episodes = null;
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

                        var url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}",
                                                   App.Settings.LoggedInUser.Id, SelectedTvSeries.Item.Id);

                        string seasonJson;
                        try
                        {
                            seasonJson = await new GZipWebClient().DownloadStringTaskAsync(url);
                        }
                        catch
                        {
                            App.ShowMessage("", "Error downloading season details");
                            return;
                        }
                        var seasons = JsonConvert.DeserializeObject<ApiBaseItemWrapper<ApiBaseItem>>(seasonJson);
                        Seasons = seasons.Children.ToList();

                        ProgressIsVisible = false;
                        ProgressText = "";
                    }
                }
            });

            SeasonPageLoaded = new RelayCommand(async () =>
            {
                if(NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !seasonDataLoaded)
                {
                    if(SelectedSeason != null)
                    {
                        
                    }
                }
            });

            EpisodePageLoaded = new RelayCommand(async ()=>
            {
                if(NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort())
                {
                    if(SelectedEpisode != null)
                    {
                        
                    }
                }
            });

            NavigateToPage = new RelayCommand<ApiBaseItemWrapper<ApiBaseItem>>(NavService.NavigateTopage);
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public ApiBaseItemWrapper<ApiBaseItem> SelectedTvSeries { get; set; }
        public List<ApiBaseItemWrapper<ApiBaseItem>> Seasons { get; set; }
        public List<ApiBaseItemWrapper<ApiBaseItem>> Episodes { get; set; }
        public ApiBaseItemWrapper<ApiBaseItem> SelectedEpisode { get; set; }
        public ApiBaseItemWrapper<ApiBaseItem> SelectedSeason { get; set; }

        public RelayCommand<ApiBaseItemWrapper<ApiBaseItem>> NavigateToPage { get; set; }
        public RelayCommand TvSeriesPageLoaded { get; set; }
        public RelayCommand SeasonPageLoaded { get; set; }
        public RelayCommand EpisodePageLoaded { get; set; }
    }
}