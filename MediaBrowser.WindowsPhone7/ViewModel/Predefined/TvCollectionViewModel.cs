using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.Services;
using MediaBrowser.WindowsPhone.Model;
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel.Predefined
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class TvCollectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _nextUpLoaded;

        /// <summary>
        /// Initializes a new instance of the TvCollectionViewModel class.
        /// </summary>
        public TvCollectionViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
        }

        public List<BaseItemDto> NextUpList { get; set; }

        public int PivotSelectedIndex { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_navigationService.IsNetworkAvailable)
                    {
                        return;
                    }


                });
            }
        }

        private async Task<bool> GetNextUp()
        {
            try
            {
                SetProgressBar("Getting next up items...");

                var query = new NextUpQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUser.Id
                };

                Log.Info("Getting next up items");

                var itemResponse = await _apiClient.GetNextUpAsync(query);

                SetItems(itemResponse, NextUpList);

                return true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetNextUp()", ex);
            }

            SetProgressBar();

            return false;
        }

        private void SetItems(ItemsResult itemResponse, List<BaseItemDto> listToSet)
        {
            if (itemResponse == null || !itemResponse.Items.Any())
            {
                return;
            }

            listToSet = itemResponse.Items.ToList();
        }
    }
}