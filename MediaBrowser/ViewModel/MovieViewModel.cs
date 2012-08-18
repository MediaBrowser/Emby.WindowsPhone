using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.DTO;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using ScottIsAFool.WindowsPhone;
using SharpGIS;
using MediaBrowser.Model.Entities;

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
        private readonly INavigationService NavService;
        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(INavigationService navService)
        {
            NavService = navService;
            if(IsInDesignMode)
            {
                SelectedMovie = new DTOBaseItem
                                    {
                                        Id = new Guid("6536a66e10417d69105bae71d41a6e6f"),
                                        Name = "Jurassic Park",
                                        SortName = "Jurassic Park",
                                        Overview = "Lots of dinosaurs eating people!"
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
                if(m.Notification.Equals(Constants.ShowMovieMsg))
                {
                    SelectedMovie = (DTOBaseItem) m.Sender;
                    ImdbId = SelectedMovie.ProviderIds["Imdb"];
                }
                else if(m.Notification.Equals(Constants.ClearFilmAndTvMsg))
                {
                    SelectedMovie = null;
                    CastAndCrew = null;
                }
            });
        }

        private void WireCommands()
        {
            MoviePageLoaded = new RelayCommand(async () =>
            {
                ProgressIsVisible = true;
                ProgressText = "Getting cast + crew...";

                bool dataLoaded = await GetMovieDetails();

                ProgressIsVisible = false;
                ProgressText = string.Empty;
            });
            NavigateTopage = new RelayCommand<DTOBaseItem>(NavService.NavigateTopage);
        }

        private async Task<bool> GetMovieDetails()
        {
            var result = false;

            string movieJson = string.Empty;
            try
            {
                string url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}", App.Settings.LoggedInUser.Id,
                                           SelectedMovie.Id);
                movieJson = await new GZipWebClient().DownloadStringTaskAsync(url);
            }
            catch
            {
                App.ShowMessage("", "Error downloading movie information");
            }

            if(!string.IsNullOrEmpty(movieJson))
            {
                var item = JsonConvert.DeserializeObject<DTOBaseItem>(movieJson);
                CastAndCrew = Utils.GroupCastAndCrew(item.People);
                result = true;
            }

            return result;
        }

        

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public DTOBaseItem SelectedMovie { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }
        public string ImdbId { get; set; }

        public RelayCommand<DTOBaseItem> NavigateTopage { get; set; }
        public RelayCommand MoviePageLoaded { get; set; }
    }
}