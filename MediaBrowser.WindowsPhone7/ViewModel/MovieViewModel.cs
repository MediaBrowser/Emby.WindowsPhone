using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.Dto;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Tasks;
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
    public class MovieViewModel : ViewModelBase
    {
        private readonly INavigationService NavService;
        private readonly ExtendedApiClient ApiClient;

        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(INavigationService navService, ExtendedApiClient apiClient)
        {
            NavService = navService;
            ApiClient = apiClient;
            CanUpdateFavourites = true;
            if (IsInDesignMode)
            {
                SelectedMovie = new BaseItemDto
                                    {
                                        Id = "6536a66e10417d69105bae71d41a6e6f",
                                        Name = "Jurassic Park",
                                        SortName = "Jurassic Park",
                                        Overview = "Lots of dinosaurs eating people!",
                                        People = new []
                                                     {
                                                         new BaseItemPerson{Name = "Steven Spielberg", Type = "Director"},
                                                         new BaseItemPerson{Name = "Sam Neill", Type = "Actor"},
                                                         new BaseItemPerson{Name = "Richard Attenborough", Type = "Actor"},
                                                         new BaseItemPerson{Name = "Laura Dern", Type = "Actor"}
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
                
            });
        }

        private void WireCommands()
        {
            MoviePageLoaded = new RelayCommand(async () =>
            {
                
                if (SelectedMovie != null && NavService.IsNetworkAvailable)
                {
                    ProgressIsVisible = true;
                    ProgressText = AppResources.SysTrayGettingMovieInfo;
                    
                    var dataLoaded = await GetMovieDetails();

                    if (SelectedMovie.ProviderIds != null)
                        ImdbId = SelectedMovie.ProviderIds["Imdb"];
                    if (SelectedMovie.RunTimeTicks.HasValue)
                        RunTime = TimeSpan.FromTicks(SelectedMovie.RunTimeTicks.Value).ToString();
                    if (SelectedMovie.UserData == null)
                        SelectedMovie.UserData = new UserItemDataDto();

                    ProgressIsVisible = false;
                    ProgressText = string.Empty;
                }
            });

            PlayMovieCommand = new RelayCommand(async () =>
                                                    {
#if WP8
                                                        Messenger.Default.Send(new NotificationMessage(SelectedMovie, Constants.PlayVideoItemMsg));
                                                        NavService.NavigateToPage("/Views/VideoPlayerView.xaml");
#else
                                                        var bounds = Application.Current.RootVisual.RenderSize;
                                                        var query = new VideoStreamOptions
                                                        {
                                                            ItemId = SelectedMovie.Id,
                                                            VideoCodec = VideoCodecs.H264,
                                                            OutputFileExtension = ".wmv",
                                                            Static = true,
                                                            AudioCodec = AudioCodecs.Mp3,
                                                            VideoBitRate = 1000000,
                                                            AudioBitRate = 128000,
                                                            MaxAudioChannels = 2,
                                                            //Profile = "baseline",
                                                            //Level = "3",
                                                            //FrameRate = 30,
                                                            MaxHeight = 480,// (int)bounds.Width,
                                                            MaxWidth = 800// (int)bounds.Height
                                                        };
                                                        var url = ApiClient.GetVideoStreamUrl(query);
                                                        System.Diagnostics.Debug.WriteLine(url);
                                                        await ApiClient.ReportPlaybackStartAsync(SelectedMovie.Id, App.Settings.LoggedInUser.Id).ConfigureAwait(true);

                                                        var mediaPlayerLauncher = new MediaPlayerLauncher
                                                                                      {
                                                                                          Orientation = MediaPlayerOrientation.Landscape,
                                                                                          Media = new Uri(url, UriKind.Absolute),
                                                                                          Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop,
                                                                                          //Location = MediaLocationType.Data
                                                                                      };
                                                        mediaPlayerLauncher.Show();
#endif
                                                    });

            AddRemoveFavouriteCommand = new RelayCommand(async () =>
            {
                try
                {
                    CanUpdateFavourites = false;
                    SelectedMovie.UserData.IsFavorite = !SelectedMovie.UserData.IsFavorite;
                    await ApiClient.UpdateFavoriteStatusAsync(SelectedMovie.Id, App.Settings.LoggedInUser.Id, SelectedMovie.UserData.IsFavorite);
                    CanUpdateFavourites = true;
                }
                catch
                {

                }
                CanUpdateFavourites = true;
            });

            ShowOtherFilmsCommand = new RelayCommand<BaseItemPerson>(person =>
                                                                         {
                                                                             App.SelectedItem = person;
                                                                             NavService.NavigateToPage("/Views/FolderView.xaml");
                                                                         });

            NavigateTopage = new RelayCommand<BaseItemDto>(NavService.NavigateToPage);
        }

        private async Task<bool> GetMovieDetails()
        {
            var result = false;

            try
            {
                var item = await ApiClient.GetItemAsync(SelectedMovie.Id, App.Settings.LoggedInUser.Id);
                SelectedMovie = item;
                CastAndCrew = Utils.GroupCastAndCrew(item.People);
                result = true;
            }
            catch
            {
                App.ShowMessage("", AppResources.ErrorGettingExtraInfo);
                result = false;
            }

            return result;
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
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