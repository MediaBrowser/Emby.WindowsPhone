using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;
using MediaBrowser.WindowsPhone.DB;
using MediaBrowser.WindowsPhone.Model;
using Wintellect.Sterling;

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

        private readonly ISterlingDatabaseInstance _databaseInstance;

        /// <summary>
        /// Initializes a new instance of the PlaylistViewModel class.
        /// </summary>
        public PlaylistViewModel(ExtendedApiClient apiClient, INavigationService navigationService, ISterlingDatabaseInstance databaseInstance)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
            _databaseInstance = databaseInstance;
            _logger = new WPLogger(typeof(PlaylistViewModel));

            Playlist = new ObservableCollection<PlaylistItem>();
            SelectedItems = new List<PlaylistItem>();
            if (IsInDesignMode)
            {
                Playlist = new ObservableCollection<PlaylistItem>
                               {
                                   new PlaylistItem {Artist = "John Williams", Album = "Jurassic Park OST", Id = 1, IsPlaying = true, TrackName = "Jurassic Park Theme"},
                                   new PlaylistItem {Artist = "John Williams", Album = "Jurassic Park OST", Id = 1, IsPlaying = true, TrackName = "Journey to the Island"}
                               };
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
                                                                                              if (m.Content == null) return;

                                                                                              m.Content.ForEach(item => _databaseInstance.Save(item));

                                                                                              GetPlaylistItems();
                                                                                          });
        }

        public ObservableCollection<PlaylistItem> Playlist { get; set; }
        public List<PlaylistItem> SelectedItems { get; set; }

        public RelayCommand PlaylistPageLoaded
        {
            get
            {
                return new RelayCommand(() =>
                                            {
                                                if (_databaseInstance == null)
                                                {
                                                    _logger.Log("Playlist database was null");
                                                    return;
                                                }

                                                GetPlaylistItems();
                                            });
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
                                                    _databaseInstance.Truncate(typeof(PlaylistItem));
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
                        var temp = Playlist.TakeWhile(x => !SelectedItems.Contains(x)).ToList();

                        foreach (var item in temp)
                        {
                            _databaseInstance.Delete(item);
                        }

                        Playlist = new ObservableCollection<PlaylistItem>(temp);
                    }
                });
            }
        }

        

        private void GetPlaylistItems()
        {
            var items = _databaseInstance.Query<PlaylistItem, int>().Select(p => p.LazyValue);

            Playlist.Clear();

            foreach (var item in items)
            {
                Playlist.Add(item.Value);
            }
        }
    }
}