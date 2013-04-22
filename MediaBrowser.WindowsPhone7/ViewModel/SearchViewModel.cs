using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.WindowsPhone.Model;

#if !WP8
using ScottIsAFool.WindowsPhone;
#endif

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
        private readonly ILog _logger;
        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel(ExtendedApiClient apiClient, INavigationService navigationService)
        {
            _apiClient = apiClient;
            _navigationService = navigationService;
            _logger = new WPLogger(typeof(SearchViewModel));

            SearchResults = new List<Group<BaseItemDto>>();

            if (IsInDesignMode)
            {
                var data = new List<BaseItemDto>
                               {
                                   new BaseItemDto
                                       {
                                           Id = "6536a66e10417d69105bae71d41a6e6f",
                                           Name = "Jurassic Park",
                                           SortName = "Jurassic Park",
                                           Overview = "Lots of dinosaurs eating people!",
                                           People = new[]
                                                        {
                                                            new BaseItemPerson {Name = "Steven Spielberg", Type = "Director"},
                                                            new BaseItemPerson {Name = "Sam Neill", Type = "Actor"},
                                                            new BaseItemPerson {Name = "Richard Attenborough", Type = "Actor"},
                                                            new BaseItemPerson {Name = "Laura Dern", Type = "Actor"}
                                                        },
                                           Type = "Movie"
                                       },
                                   new BaseItemDto
                                       {
                                           Id = "e252ea3059d140a0274282bc8cd194cc",
                                           Name = "1x01 - Pilot",
                                           SortName = "1x01 - Pilot",
                                           Overview =
                                               "A Kindergarten teacher starts speaking gibberish and passed out in front of her class. What looks like a possible brain tumor does not respond to treatment and provides many more questions than answers for House and his team as they engage in a risky trial-and-error approach to her case. When the young teacher refuses any additional variations of treatment and her life starts slipping away, House must act against his code of conduct and make a personal visit to his patient to convince her to trust him one last time.",
                                           Type = "Episode"
                                       }
                               };
                GroupSearchResults(data);
            }
            else
            {
                WireMessages();
            }
        }

        private void WireMessages()
        {

        }

        private async Task DoSearch()
        {
            try
            {
                _logger.LogFormat("Searching for [{0}]", LogLevel.Info, SearchText);

                //var items = await _apiClient.

                GroupSearchResults(new List<BaseItemDto>());
            }
            catch (HttpException ex)
            {
                _logger.Log(ex.Message, LogLevel.Fatal);
                _logger.Log(ex.StackTrace, LogLevel.Fatal);

                App.ShowMessage("", "Failed to run search");
            }
        }

        private void GroupSearchResults(List<BaseItemDto> items)
        {
            if (items == null || !items.Any()) return;

            var emptyGroups = new List<Group<BaseItemDto>>();

            var types = items.Select(x => x.Type).Distinct().ToList();

            types.ForEach(type => emptyGroups.Add(new Group<BaseItemDto>(type, new List<BaseItemDto>())));

            var groupedItems = (from t in items
                                group t by t.Type
                                into grp
                                orderby grp.Key
                                select new Group<BaseItemDto>(grp.Key, grp)).ToList();
#if WP8
            SearchResults = groupedItems;
#else
            SearchResults = (from g in groupedItems.Union(emptyGroups)
                             orderby g.Title
                             select g).ToList();
#endif
        }

        // UI Properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public string SearchText { get; set; }

        public List<Group<BaseItemDto>> SearchResults { get; set; }

        public RelayCommand DoSearchCommand
        {
            get
            {
                return new RelayCommand(async () =>
                                            {
                                                if (string.IsNullOrEmpty(SearchText)) return;

                                                await DoSearch();
                                            });
            }
        }
    }
}