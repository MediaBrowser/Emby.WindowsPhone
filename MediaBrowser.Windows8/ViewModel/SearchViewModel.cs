using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Net;
using MediaBrowser.Windows8.Model;
using MediaBrowser.Windows8.Views;
using MetroLog;
using ReflectionIT.Windows8.Helpers;
using Windows.UI.Xaml;

namespace MediaBrowser.Windows8.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly ExtendedApiClient _apiClient;
        private readonly ILogger _logger;

        private bool _dataLoaded;
        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel(NavigationService navigationService, ExtendedApiClient apiClient)
        {
            _navigationService = navigationService;
            _apiClient = apiClient;
            _logger = LogManagerFactory.DefaultLogManager.GetLogger<SearchViewModel>();

            if (IsInDesignMode)
            {
                SearchTerm = "jurassic park";
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
                SearchResults = data;
                FilterItems(new SearchView.Filter("All", 10, true));
                FilterList.Add(new SearchView.Filter("Movie", 0));
                FilterList.Add(new SearchView.Filter("Episode", 0));
                FilterList.Add(new SearchView.Filter("TV Show", 0));
            }
            else
            {
                WireMessages();
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
                                                                      {
                                                                          if (m.Notification.Equals(Constants.SearchPageLoadedMsg))
                                                                          {
                                                                              await DoSearch(m.Sender.ToString());
                                                                          }

                                                                          if (m.Notification.Equals(Constants.ChangeFilteredResultsMsg))
                                                                          {
                                                                              if (m.Target.ToString().Equals(SearchTerm))
                                                                              {
                                                                                  FilterItems((SearchView.Filter)m.Sender);
                                                                              }
                                                                          }
                                                                      });
        }

        private async Task DoSearch(string searchTerm)
        {
            if (_navigationService.IsNetworkAvailable)
            {
                if (!_dataLoaded && searchTerm.Equals(SearchTerm))
                {
                    ProgressText = "Searching...";
                    ProgressVisibility = Visibility.Visible;

                    _logger.Info("Searching for [{0}]", searchTerm);

                    try
                    {
                        //var items = _apiClient.Search
                        var items = new List<BaseItemDto>();

                        if (items.Any())
                        {
                            CreateFilter(items);
                            
                            SearchResults = items;

                            FilterItems(FilterList[0]);

                            _dataLoaded = true;
                        }
                        else
                        {
                            Messenger.Default.Send(new NotificationMessage(Constants.NoResultsMsg));
                        }
                    }
                    catch (HttpException ex)
                    {
                        _logger.Fatal("DoSearch()", ex);
                    }
                }
            }
            else
            {
                await MessageBox.ShowAsync("You appear to have no internet connection.",
                                           "No internet",
                                           MessageBoxButton.OK);
            }
        }

        private void CreateFilter(IReadOnlyCollection<BaseItemDto> items)
        {
            FilterList.SingleOrDefault(x => x.Name.Equals("All")).Count = items.Count;

            foreach (var item in items)
            {
                var item1 = item;
                var filter = FilterList.FirstOrDefault(x => x.Name.Equals(item1.Type));
                if (filter != null)
                {
                    filter.Count++;
                }
                else
                {
                    filter = new SearchView.Filter(item.Type, 1);
                    FilterList.Add(filter);
                }
            }
        }

        private void FilterItems(SearchView.Filter filter)
        {
            if (filter.Name.Equals("All"))
            {
                FilteredResults = SearchResults;
                return;
            }
            var items = SearchResults.Where(x => x.Type.Equals(filter.Name)).ToList();
            FilteredResults = items;
            if (!FilteredResults.Any())
                Messenger.Default.Send(new NotificationMessage(SearchTerm, Constants.NoResultsMsg));
        }

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }

        public string SearchTerm { get; set; }

        public List<BaseItemDto> SearchResults { get; set; }
        public ICollection<BaseItemDto> FilteredResults { get; set; }
        public ObservableCollection<SearchView.Filter> FilterList { get; set; }
    }
}