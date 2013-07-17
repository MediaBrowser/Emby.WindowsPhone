using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Search;
using MediaBrowser.WindowsPhone.Model;
#if !WP8
using ScottIsAFool.WindowsPhone;
#endif
using ScottIsAFool.WindowsPhone.ViewModel;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        private readonly ExtendedApiClient _apiClient;
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;

            SearchResults = new List<Group<SearchHint>>();

            if (IsInDesignMode)
            {
                var data = new List<SearchHint>
                {
                    new SearchHint
                    {
                        Name = "Jurassic Park",
                        Type = "Movie"
                    },
                    new SearchHint
                    {
                        Name = "1x01 - Pilot",
                        Type = "Episode"
                    }
                };
                GroupSearchResults(data);
            }
        }

        private async Task DoSearch()
        {
            try
            {
                Log.Info("Searching for [{0}]", SearchText);

                var items = await _apiClient.GetSearchHints(App.Settings.LoggedInUser.Id, SearchText, null, null);

                if (items != null)
                {
                    GroupSearchResults(items.SearchHints.ToList());
                }
            }
            catch (HttpException ex)
            {
                Log.ErrorException("DoSearch()", ex);

                App.ShowMessage("Failed to run search");
            }
        }

        private void GroupSearchResults(List<SearchHint> items)
        {
            if (items == null || !items.Any()) return;

            var emptyGroups = new List<Group<SearchHint>>();

            var types = items.Select(x => x.Type).Distinct().ToList();

            types.ForEach(type => emptyGroups.Add(new Group<SearchHint>(type, new List<SearchHint>())));

            var groupedItems = (from t in items
                group t by t.Type
                into grp
                orderby grp.Key
                select new Group<SearchHint>(grp.Key, grp)).ToList();
#if WP8
            SearchResults = (from g in groupedItems.Union(emptyGroups)
                orderby g.Title
                select g).ToList();
#else
            SearchResults = (from g in groupedItems.Union(emptyGroups)
                             orderby g.Title
                             select g).ToList();
#endif
        }

        public string SearchText { get; set; }

        public List<Group<SearchHint>> SearchResults { get; set; }

        public RelayCommand DoSearchCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        return;
                    }

                    await DoSearch();
                });
            }
        }
    }
}