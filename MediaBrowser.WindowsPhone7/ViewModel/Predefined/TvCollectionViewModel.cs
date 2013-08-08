using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JetBrains.Annotations;
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

            if (IsInDesignMode)
            {
                NextUpList = new List<BaseItemDto>
                {
                    new BaseItemDto
                    {
                        Id = "e252ea3059d140a0274282bc8cd194cc",
                        Name = "1x01 - Pilot",
                        Overview =
                            "A Kindergarten teacher starts speaking gibberish and passed out in front of her class. What looks like a possible brain tumor does not respond to treatment and provides many more questions than answers for House and his team as they engage in a risky trial-and-error approach to her case. When the young teacher refuses any additional variations of treatment and her life starts slipping away, House must act against his code of conduct and make a personal visit to his patient to convince her to trust him one last time.",
                        SeriesName = "House M.D."
                    }
                };
            }
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

                    _nextUpLoaded = await GetNextUp();
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
                
                return SetNextUpItems(itemResponse);
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetNextUp()", ex);
            }

            SetProgressBar();

            return false;
        }

        private bool SetNextUpItems(ItemsResult itemResponse)
        {
            if (itemResponse == null || !itemResponse.Items.Any())
            {
                return false;
            }

            NextUpList = itemResponse.Items.ToList();

            SetProgressBar();

            return true;
        }

        [UsedImplicitly]
        private void OnPivotSelectedIndexChanged()
        {
            
        }
    }
}