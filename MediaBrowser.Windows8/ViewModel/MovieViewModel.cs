using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MediaBrowser.Windows8.Views;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MovieViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient ApiClient;
        private readonly NavigationService NavigationService;
        private bool dataLoaded;
        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(ExtendedApiClient apiClient, NavigationService navigationService)
        {
            
            MediaStreams = new List<Group<MediaStream>>();
            if (IsInDesignMode)
            {
                CastAndCrew = new ObservableCollection<Group<BaseItemPerson>>
                              {
                                  new Group<BaseItemPerson> {Title = "Director"},
                                  new Group<BaseItemPerson> {Title = "Cast"}
                              };
                SelectedMovie = new BaseItemDto
                                    {
                                        Id = "969072baf139763483c275b34d69e8c4",
                                        Name = "Jurassic Park",
                                        Overview = "Lots of dinosaurs eating everyone!! Oh noes",
                                        OfficialRating = "PG-13",
                                        Path = @"g:\jurassic park.mkv",
                                        AspectRatio = "16:9",
                                        ProductionYear = 1993,
                                        DisplayMediaType = "MKV",
                                        RunTimeTicks = 67857820000,
                                        Genres = new[] { "Adventure", "Family", "Sci-Fi", "Science Fiction" }.ToList(),
                                        Taglines = new[] { "64 million years in the making" }.ToList(),
                                        MediaStreams = new List<MediaStream>
                                                           {
                                                               new MediaStream
                                                                   {
                                                                       Type = MediaStreamType.Video,
                                                                       Height = 720,
                                                                       Language = "eng"
                                                                   },
                                                                   new MediaStream
                                                                   {
                                                                       Type = MediaStreamType.Audio,
                                                                       Language = "eng"
                                                                   },
                                                                   new MediaStream
                                                                   {
                                                                       Type = MediaStreamType.Subtitle,
                                                                       Language = "eng"
                                                                   },
                                                                   new MediaStream
                                                                   {
                                                                       Type = MediaStreamType.Subtitle,
                                                                       Language = "fra"
                                                                   }
                                                           },
                                        UserData = new UserItemDataDto
                                                       {
                                                           IsFavorite = false
                                                       }
                                    };
                GroupMediaStreams();
                CastAndCrew[0].Items.Add(new BaseItemPerson { Name = "Steven Spielberg", Role = "The main guy" });
                CastAndCrew[1].Items.Add(new BaseItemPerson { Name = "Sam Neill", Role = "Dr Alan Grant" });
                CastAndCrew[1].Items.Add(new BaseItemPerson { Name = "Richard Attenborough", Role = "John Hammond" });
            }
            else
            {
                ProgressVisibility = Visibility.Collapsed;
                ApiClient = apiClient;
                NavigationService = navigationService;
                WireMessages();
                WireCommands();
                CanUpdateFavourites = true;
            }

        }

        private void GroupMediaStreams()
        {
            if (SelectedMovie.MediaStreams != null)
            {
                var groups = SelectedMovie.MediaStreams
                                          .GroupBy(x => x.Type)
                                          .Select(m => new Group<MediaStream>
                                                           {
                                                               Title = m.Key.ToString(),
                                                               Items = new ObservableCollection<MediaStream>(m.ToList())
                                                           }).ToList();
                MediaStreams = groups;

            }
        }

        private void WireCommands()
        {
            GenreTappedCommand = new RelayCommand<ItemClickEventArgs>(args =>
                                                                          {
                                                                              // Navigate to the Folder view
                                                                              var genreFolder = new BaseItemDto
                                                                                                    {
                                                                                                        Name = (string)args.ClickedItem,
                                                                                                        Type = "genre"
                                                                                                    };
                                                                              NavigationService.NavigateToPage(genreFolder);
                                                                          });

            BackButtonCommand = new RelayCommand(() =>
                                                     {

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

            LikeDislikeCommand = new RelayCommand<string>(async isLike =>
                                                                    {
                                                                        try
                                                                        {
                                                                            SelectedMovie.UserData.Likes = bool.Parse(isLike);
                                                                            await ApiClient.UpdateUserItemRatingAsync(SelectedMovie.Id, App.Settings.LoggedInUser.Id, bool.Parse(isLike));
                                                                        }
                                                                        catch (HttpException ex)
                                                                        {
                                                                            var v = "v";
                                                                        }
                                                                    });

            
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.MovieViewLoadedMsg))
                {
                    var id = (string)m.Sender;
                    ProgressText = "Getting cast and crew...";
                    ProgressVisibility = Visibility.Visible;
                    if (SelectedMovie != null && NavigationService.IsNetworkAvailable && (SelectedMovie.Id == id) && !dataLoaded)
                    {

                        var item = await ApiClient.GetItemAsync(SelectedMovie.Id, App.Settings.LoggedInUser.Id);
                        if (item.UserData == null) item.UserData = new UserItemDataDto();
                        SelectedMovie = item;

                        CastAndCrew = await Utils.GroupCastAndCrew(item);
                        dataLoaded = true;
                        GroupMediaStreams();
                    }


                    ProgressText = "";
                    ProgressVisibility = Visibility.Collapsed;
                }
                if (m.Notification.Equals(Constants.MovieViewBackMsg))
                {
                    //var vm = History.Current.GetItem<MovieViewModel>(SelectedMovie.Id);
                    //CastAndCrew = vm.CastAndCrew;
                    //CastWidth = vm.CastWidth;
                    //Tagline = vm.Tagline;
                }
                if (m.Notification.Equals(Constants.ClearEverythingMsg))
                {
                    Reset();
                }
            });
        }

        private void Reset()
        {
            CastAndCrew[0].Items.Clear();
            CastAndCrew[1].Items.Clear();
            SelectedMovie = null;
        }

        public ObservableCollection<Group<BaseItemPerson>> CastAndCrew { get; set; }
        public List<Group<MediaStream>> MediaStreams { get; set; }
        public BaseItemDto SelectedMovie { get; set; }
        public string Tagline { get; set; }
        public double CastWidth { get; set; }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }
        public bool CanUpdateFavourites { get; set; }

        public RelayCommand<ItemClickEventArgs> GenreTappedCommand { get; set; }
        public RelayCommand BackButtonCommand { get; set; }
        public RelayCommand AddRemoveFavouriteCommand { get; set; }
        public RelayCommand<string> LikeDislikeCommand { get; set; }
    }
}