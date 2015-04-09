using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Search;
using Emby.WindowsPhone.Extensions;
using Emby.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using Emby.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

namespace Emby.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SearchViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SearchViewModel class.
        /// </summary>
        public SearchViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base(navigationService, connectionManager)
        {
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
                SetProgressBar(AppResources.SysTraySearching);

                var query = new SearchQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SearchTerm = SearchText
                };

                var items = await ApiClient.GetSearchHintsAsync(query);

                if (items != null)
                {
                    await GroupSearchResults(items.SearchHints.ToList());
                }

                SetProgressBar();
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("DoSearch()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorFailedToSearch);
            }
        }

        private async Task GroupSearchResults(List<SearchHint> items)
        {
            if (items.IsNullOrEmpty())
            {
                return;
            }

            await Task.Run(() =>
            {
                var emptyGroups = new List<Group<SearchHint>>();

                var types = items.Select(x => x.Type).Distinct().ToList();

                types.ForEach(type => emptyGroups.Add(new Group<SearchHint>(type, new List<SearchHint>())));

                var groupedItems = (from t in items
                    group t by t.Type
                    into grp
                    orderby grp.Key
                    select new Group<SearchHint>(grp.Key, grp)).ToList();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    SearchResults = (from g in groupedItems.Union(emptyGroups)
                        where g.Count > 0
                        orderby g.Title
                        select g).ToList();
                });
            });
        }

        public string SearchText { get; set; }

        public List<Group<SearchHint>> SearchResults { get; set; }

        public RelayCommand DoSearchCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (string.IsNullOrEmpty(SearchText) || !NavigationService.IsNetworkAvailable)
                    {
                        return;
                    }

                    await DoSearch();
                });
            }
        }

        public RelayCommand<SearchHint> SearchItemTappedCommand
        {
            get
            {
                return new RelayCommand<SearchHint>(item => NavigationService.NavigateTo(item.ToBaseItemDto()));
            }
        }
    }
}