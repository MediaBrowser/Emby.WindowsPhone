﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Extensions;
using Emby.WindowsPhone.AudioAgent;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.ApiInteraction.Playback;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Playlists;
using MediaBrowser.Model.Querying;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Messaging;
using Emby.WindowsPhone.Model;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using Microsoft.Phone.Controls;

using CustomMessageBox = Emby.WindowsPhone.Controls.CustomMessageBox;

namespace Emby.WindowsPhone.ViewModel.Playlists
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ServerPlaylistsViewModel : ViewModelBase
    {
        private readonly IPlaybackManager _playbackManager;
        private bool _playlistLoaded;

        /// <summary>
        /// Initializes a new instance of the ServerPlaylistsViewModel class.
        /// </summary>
        public ServerPlaylistsViewModel(INavigationService navigationService, IConnectionManager connectionManager, IPlaybackManager playbackManager)
            : base(navigationService, connectionManager)
        {
            _playbackManager = playbackManager;
            if (IsInDesignMode)
            {
                SelectedPlaylist = new BaseItemDto
                {
                    Name = "Jurassic Park"
                };
            }
        }

        public BaseItemDto SelectedPlaylist { get; set; }
        public ObservableCollection<BaseItemDto> PlaylistItems { get; set; }

        public ObservableCollection<BaseItemDto> SelectedItems { get; set; }

        public bool CanDeleteItems
        {
            get
            {
                return !ProgressIsVisible
                       && !SelectedItems.IsNullOrEmpty();
            }
        }

        public bool IsInSelectionMode { get; set; }
        public int SelectedIndex { get { return IsInSelectionMode ? 1 : 0; } }

        public string NumberOfItems
        {
            get
            {
                if (PlaylistItems.IsNullOrEmpty())
                {
                    return AppResources.LabelNoItems;
                }

                if (PlaylistItems.Count == 1)
                {
                    return AppResources.LabelOneItem;
                }

                return string.Format(AppResources.LabelMultipleItems, PlaylistItems.Count);
            }
        }

        public RelayCommand PlaylistViewLoaded
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        private async Task LoadData(bool isRefresh)
        {
            if (SelectedPlaylist == null ||
                !NavigationService.IsNetworkAvailable
                || (_playlistLoaded && !isRefresh))
            {
                return;
            }

            try
            {
                SetProgressBar(AppResources.SysTrayGettingDetails);
                
                var query = new PlaylistItemQuery
                {
                    Id = SelectedPlaylist.Id,
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    Fields = new[] { ItemFields.MediaSources, ItemFields.SyncInfo }
                };
                var items = await ApiClient.GetPlaylistItems(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    PlaylistItems = items.Items.ToObservableCollection();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "LoadData(" + isRefresh + ")", NavigationService, Log);
            }

            SetProgressBar();
        }

        public RelayCommand<BaseItemDto> ItemTappedCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    if (SelectedPlaylist.MediaType.Equals("Video"))
                    {
                        var message = new VideoMessage(PlaylistItems, item, false);
                        if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null)
                        {
                            Messenger.Default.Send(message);
                            NavigationService.NavigateTo(Constants.Pages.VideoPlayerView);
                        }
                    }
                    else if (SelectedPlaylist.MediaType.Equals("Audio"))
                    {
                        var playlistItem = await item.ToPlaylistItem(ApiClient, _playbackManager);
                        var tracks = new List<PlaylistItem> { playlistItem };

                        Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(tracks, Constants.Messages.SetPlaylistAsMsg));
                    }
                });
            }
        }

        public RelayCommand StartPlaylistCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (PlaylistItems.IsNullOrEmpty())
                    {
                        return;
                    }

                    var items = PlaylistItems;
                    await StartPlaylist(items);
                });
            }
        }

        public RelayCommand StartShufflePlayCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (PlaylistItems.IsNullOrEmpty())
                    {
                        return;
                    }

                    var randomisedList = PlaylistItems.Randomise();

                    await StartPlaylist(randomisedList);
                });
            }
        }

        public RelayCommand DeleteItemsFromPlaylistCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (SelectedItems.IsNullOrEmpty())
                    {
                        return;
                    }

                    var messageBox = new CustomMessageBox
                    {
                        Title = AppResources.MessageAreYouSureTitle,
                        Message = AppResources.MessageDeletePlaylistItems,
                        LeftButtonContent = AppResources.LabelYes,
                        RightButtonContent = AppResources.LabelNo
                    };
                    var result = await messageBox.ShowAsync();

                    if (result == CustomMessageBoxResult.RightButton)
                    {
                        return;
                    }

                    var itemIds = SelectedItems.Select(x => x.PlaylistItemId);
                    var cumulativeRuntime = SelectedItems.Sum(x => x.RunTimeTicks ?? 0);
                    try
                    {
                        SetProgressBar(AppResources.SysTrayRemoving);

                        await ApiClient.RemoveFromPlaylist(SelectedPlaylist.Id, itemIds);

                        foreach (var item in SelectedItems.ToList())
                        {
                            var removeItem = PlaylistItems.FirstOrDefault(x => x.Id == item.Id);
                            if (removeItem != null)
                            {
                                PlaylistItems.Remove(removeItem);
                            }
                        }

                        RaisePropertyChanged(() => NumberOfItems);
                        if (SelectedPlaylist.CumulativeRunTimeTicks.HasValue)
                        {
                            SelectedPlaylist.CumulativeRunTimeTicks = SelectedPlaylist.CumulativeRunTimeTicks.Value - cumulativeRuntime;
                        }

                        SelectedItems.Clear();
                        IsInSelectionMode = false;
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "DeleteItemsFromPlaylist", NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand<BaseItemDto> StartInstantMixCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    SetProgressBar(AppResources.SysTrayGettingInstantMix);

                    try
                    {
                        var tracks = await ApiClient.GetInstantMixPlaylist(item, _playbackManager);
                        Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(tracks, Constants.Messages.SetPlaylistAsMsg));
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "StartInstantMix", NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        public RelayCommand<BaseItemDto> DeleteFromPlaylistCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(async item =>
                {
                    if (item == null)
                    {
                        return;
                    }

                    try
                    {
                        SetProgressBar(AppResources.SysTrayRemoving);

                        await ApiClient.RemoveFromPlaylist(SelectedPlaylist.Id, new List<string> {item.Id});
                        PlaylistItems.Remove(item);
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException(ex, "DeleteFromPlaylist", NavigationService, Log);
                    }

                    SetProgressBar();
                });
            }
        }

        private async Task StartPlaylist(IList<BaseItemDto> items)
        {
            if (SelectedPlaylist.MediaType.Equals("Video"))
            {
                var message = new VideoMessage(items, items.First(), false);
                if (SimpleIoc.Default.GetInstance<VideoPlayerViewModel>() != null)
                {
                    Messenger.Default.Send(message);
                    NavigationService.NavigateTo(Constants.Pages.VideoPlayerView);
                }
            }
            else if (SelectedPlaylist.MediaType.Equals("Audio"))
            {
                var tracks = await items.ToList().ToPlayListItems(ApiClient, _playbackManager);

                Messenger.Default.Send(new NotificationMessage<List<PlaylistItem>>(tracks, Constants.Messages.SetPlaylistAsMsg));
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ServerPlaylistChangedMsg))
                {
                    SelectedPlaylist = m.Sender as BaseItemDto;
                    ServerIdItem = SelectedPlaylist;
                    PlaylistItems = null;
                    _playlistLoaded = false;
                }
            });
        }

        public override void UpdateProperties()
        {
            base.UpdateProperties();
            RaisePropertyChanged(() => CanDeleteItems);
        }
    }
}