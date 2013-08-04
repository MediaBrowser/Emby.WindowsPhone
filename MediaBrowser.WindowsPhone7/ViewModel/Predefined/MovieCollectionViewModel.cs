using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
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
    public class MovieCollectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IExtendedApiClient _apiClient;

        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the MovieCollectionViewModel class.
        /// </summary>
        public MovieCollectionViewModel(INavigationService navigationService, IExtendedApiClient apiClient)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            if (IsInDesignMode)
            {
                Genres = new List<BaseItemDto>
                {
                    new BaseItemDto {Name = "Action", Type = "Genre"}
                };
            }
        }

        public List<BaseItemDto> Genres { get; set; }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!_navigationService.IsNetworkAvailable || _dataLoaded)
                    {
                        return;
                    }

                    await GetGenres();
                });
            }
        }

        public RelayCommand<BaseItemDto> NavigateToCommand
        {
            get
            {
                return new RelayCommand<BaseItemDto>(_navigationService.NavigateTo);
            }
        }

        private async Task GetGenres()
        {
            var query = new ItemsByNameQuery
            {
                SortBy = new[] {"SortName"},
                SortOrder = SortOrder.Ascending,
                IncludeItemTypes = new[] {"Movie"},
                Recursive = true,
                Fields = new[] {ItemFields.ItemCounts, ItemFields.DateCreated}
            };

            try
            {
                var genresResponse = await _apiClient.GetGenresAsync(query);

                if (genresResponse != null)
                {
                    Genres = genresResponse.Items.ToList();
                }

                _dataLoaded = true;
            }
            catch (HttpException ex)
            {
                Log.ErrorException("GetGenres()", ex);
            }
        }
    }
}