using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Resources;
using ScottIsAFool.WindowsPhone;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TrailerViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the TrailerViewModel class.
        /// </summary>
        public TrailerViewModel(INavigationService navigation, IExtendedApiClient apiClient)
        {
            _navigationService = navigation;
            _apiClient = apiClient;

            if (IsInDesignMode)
            {
                SelectedTrailer = new BaseItemDto
                {
                    Name = "Jurassic Park 3D",
                    Overview =
                        "Universal Pictures will release Steven Spielberg\u2019s groundbreaking masterpiece JURASSIC PARK in 3D on April 5, 2013.  With his remastering of the epic into a state-of-the-art 3D format, Spielberg introduces the three-time Academy Award\u00AE-winning blockbuster to a new generation of moviegoers and allows longtime fans to experience the world he envisioned in a way that was unimaginable during the film\u2019s original release.  Starring Sam Neill, Laura Dern, Jeff Goldblum, Samuel L. Jackson and Richard Attenborough, the film based on the novel by Michael Crichton is produced by Kathleen Kennedy and Gerald R. Molen.",
                    PremiereDate = DateTime.Parse("2013-04-05T00:00:00.0000000"),
                    Id = "4aed3d79a0c4c2a0ac9c91fb7a641f1a",
                    ProductionYear = 2013,
                    People = new[]
                    {
                        new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                        new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                        new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                        new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                    }
                };
                CastAndCrew = Utils.GroupCastAndCrew(SelectedTrailer.People);
            }
            else
            {
                WireCommands();
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ChangeTrailerMsg))
                {
                    SelectedTrailer = (BaseItemDto) App.SelectedItem;
                    _dataLoaded = false;
                }
            });
        }

        private void WireCommands()
        {
            TrailerPageLoaded = new RelayCommand(async () =>
            {
                if (_navigationService.IsNetworkAvailable && !_dataLoaded)
                {
                    SetProgressBar(AppResources.SysTrayGettingTrailerDetails);

                    try
                    {
                        Log.Info("Getting information for trailer [{0}] ({1})", SelectedTrailer.Name, SelectedTrailer.Id);

                        SelectedTrailer = await _apiClient.GetItemAsync(SelectedTrailer.Id, AuthenticationService.Current.LoggedInUser.Id);

                        CastAndCrew = Utils.GroupCastAndCrew(SelectedTrailer.People);

                        _dataLoaded = true;
                    }
                    catch (HttpException ex)
                    {
                        Log.ErrorException("TrailerPageLoaded", ex);

                        App.ShowMessage(AppResources.ErrorGettingTrailerDetails);
                    }

                    SetProgressBar();
                }
            });
        }

        public BaseItemDto SelectedTrailer { get; set; }
        public List<Group<BaseItemPerson>> CastAndCrew { get; set; }

        public RelayCommand TrailerPageLoaded { get; set; }
    }
}