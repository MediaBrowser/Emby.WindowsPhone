using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cimbalino.Phone.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;
using MediaBrowser.WindowsPhone.Model;
using INavigationService = MediaBrowser.WindowsPhone.Model.INavigationService;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PlaylistViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;
        private readonly ILog _logger;
        private readonly IApplicationSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the PlaylistViewModel class.
        /// </summary>
        public PlaylistViewModel(ExtendedApiClient apiClient, INavigationService navigationService, IApplicationSettingsService applicationSettingsService)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
            _logger = new WPLogger(typeof(PlaylistViewModel));
            _settingsService = applicationSettingsService;

            Playlist = new ObservableCollection<PlaylistItem>();
            SelectedItems = new List<PlaylistItem>();
            if (IsInDesignMode)
            {
                Playlist = new ObservableCollection<PlaylistItem>
                               {
                                   new PlaylistItem {Artist = "John Williams", Album = "Jurassic Park OST", Id = 1, IsPlaying = true, TrackName = "Jurassic Park Theme"},
                                   new PlaylistItem {Artist = "John Williams", Album = "Jurassic Park OST", Id = 2, IsPlaying = false, TrackName = "Journey to the Island"},
                                   new PlaylistItem {Artist = "John Williams", Album = "Jurassic Park OST", Id = 3, IsPlaying = false, TrackName = "Incident at Isla Nublar"}
                               };
                NowPlayingItem = Playlist[0];
            }
            else
            {
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage<List<PlaylistItem>>>(this, m =>
                                                                                          {
                                                                                              if (m.Notification.Equals(Constants.AddToPlaylistMsg))
                                                                                              {
                                                                                                  AddToPlaylist(m.Content);
                                                                                              }
                                                                                          });
        }

        private void AddToPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var items = _settingsService.Get<List<PlaylistItem>>(Constants.CurrentPlaylist);

            _logger.LogFormat("Adding {0} item(s) to the playlist", LogLevel.Info, list.Count);

            list.ForEach(items.Add);

            ResetTrackNumbers(list);

            _settingsService.Set(Constants.CurrentPlaylist, list);
            _settingsService.Save();
        }

        private void RemoveFromPlaylist(List<PlaylistItem> list)
        {
            if (list == null || !list.Any()) return;

            var items = _settingsService.Get<List<PlaylistItem>>(Constants.CurrentPlaylist);

            var afterRemoval = items.Where(x => !list.Contains(x)).ToList();

            ResetTrackNumbers(afterRemoval);
        }

        public ObservableCollection<PlaylistItem> Playlist { get; set; }
        public List<PlaylistItem> SmallList
        {
            get
            {
                return Playlist.Where(x => !x.IsPlaying)
                               .OrderBy(x => x.Id)
                               .Take(3)
                               .ToList();
            }
        }
        public List<PlaylistItem> SelectedItems { get; set; }
        public PlaylistItem NowPlayingItem { get; set; }

        public bool IsInSelectionMode { get; set; }
        public int SelectedAppBarIndex { get { return IsInSelectionMode ? 1 : 0; } }

        public RelayCommand PlaylistPageLoaded
        {
            get
            {
                return new RelayCommand(GetPlaylistItems);
            }
        }

        public RelayCommand ClearPlaylistCommand
        {
            get
            {
                return new RelayCommand(() =>
                                            {
                                                var result = MessageBox.Show("Are you sure you want to clear your playlist?", "Are you sure?", MessageBoxButton.OKCancel);
                                                
                                                if (result == MessageBoxResult.OK)
                                                {
                                                    _settingsService.Reset(Constants.CurrentPlaylist);

                                                    GetPlaylistItems();
                                                }
                                            });
            }
        }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand
        {
            get
            {
                return new RelayCommand<SelectionChangedEventArgs>(args =>
                {
                    if (args.AddedItems != null)
                    {
                        foreach (var item in args.AddedItems.Cast<PlaylistItem>())
                        {
                            SelectedItems.Add(item);
                        }
                    }

                    if (args.RemovedItems != null)
                    {
                        foreach (var item in args.RemovedItems.Cast<PlaylistItem>())
                        {
                            SelectedItems.Remove(item);
                        }
                    }
                    RaisePropertyChanged(() => SelectedItems);
                });
            }
        }

        public RelayCommand DeleteItemsCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var result = MessageBox.Show("Are you sure you wish to delete these items? This cannot be undone.", "Are you sure?", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        RemoveFromPlaylist(SelectedItems);

                        IsInSelectionMode = false;

                        GetPlaylistItems();
                    }
                });
            }
        }

        private void ResetTrackNumbers(IEnumerable<PlaylistItem> list)
        {
            if (list == null) return;

            var i = 1;
            foreach (var item in list)
            {
                item.Id = i;
                i++;
            }

            _settingsService.Set(Constants.CurrentPlaylist, list.ToList());
            _settingsService.Save();
        }

        private void GetPlaylistItems()
        {
            var items = _settingsService.Get<List<PlaylistItem>>(Constants.CurrentPlaylist);

            if (items == null) return;

            Playlist = new ObservableCollection<PlaylistItem>(items);

            var nowPlaying = items.FirstOrDefault(x => x.IsPlaying);
            if (nowPlaying != null) NowPlayingItem = nowPlaying;
        }
    }
}