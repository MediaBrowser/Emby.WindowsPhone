using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using MediaBrowser.WindowsPhone.Resources;
using MediaBrowser.WindowsPhone.Services;
using ScottIsAFool.WindowsPhone;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class FolderViewModel : ViewModelBase
    {
        private bool _dataLoaded;

        /// <summary>
        /// Initializes a new instance of the FolderViewModel class.
        /// </summary>
        public FolderViewModel(INavigationService navigationService, IConnectionManager connectionManager)
            : base(navigationService, connectionManager)
        {
            RecentItems = new ObservableCollection<BaseItemDto>();
            RandomItems = new ObservableCollection<BaseItemDto>();

            if (IsInDesignMode)
            {
                SelectedFolder = new BaseItemDto
                {
                    Name = "Movies"
                };
                RecentItems.Add(new BaseItemDto
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
                    }

                });
            }
            else
            {
                WireCommands();
                GroupBy = GroupBy.Name;
            }
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShowFolderMsg))
                {
                    SelectedFolder = m.Sender as BaseItemDto;
                    _dataLoaded = false;
                }
                else if (m.Notification.Equals(Constants.Messages.ChangeGroupingMsg))
                {
                    GroupBy = (GroupBy) m.Sender;
                    SortList();
                }
                else if (m.Notification.Equals(Constants.Messages.ClearFoldersMsg))
                {
                    CurrentItems = null;
                    FolderGroupings = null;
                }
                else if (m.Notification.Equals(Constants.Messages.CollectionPinnedMsg))
                {
                    var tileUrl = (string) m.Sender;
                    CanPinCollection = TileService.Current.TileExists(tileUrl);
                }
            });
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavigationService.IsNetworkAvailable && !_dataLoaded)
                {
                    SetProgressBar(AppResources.SysTrayGettingItems);

                    GroupBy = App.SpecificSettings.DefaultGroupBy;
                    _dataLoaded = await GetItems();

                    SortList();

                    SetProgressBar();
                }
            });

            CollectionPageLoaded = new RelayCommand(async () =>
            {
                if (NavigationService.IsNetworkAvailable && !_dataLoaded && SelectedFolder != null)
                {
                    SetProgressBar(AppResources.SysTrayCheckingCollection);

                    var tileUrl = string.Format(Constants.PhoneTileUrlFormat, "Collection", SelectedFolder.Id, SelectedFolder.Name);

                    CanPinCollection = TileService.Current.TileExists(tileUrl);

                    _dataLoaded = await GetCollectionItems();

                    SetProgressBar();
                }

                if (!CurrentItems.IsNullOrEmpty())
                {
                    GetRandomItems();
                }
            });

            SeeMoreCommand = new RelayCommand(() =>
            {
                App.SelectedItem = SelectedFolder;
                NavigationService.NavigateTo("/Views/FolderView.xaml");
            });

            NavigateTo = new RelayCommand<BaseItemDto>(NavigationService.NavigateTo);
        }

        private void GetRandomItems()
        {
            RandomItems.Clear();
            var random = CurrentItems.OrderBy(n => Guid.NewGuid()).Take(6).ToList();
            random.ForEach(item => RandomItems.Add(item));
        }

        private async Task<bool> GetCollectionItems()
        {
            var getItems = await GetItems();

            var getRecent = await GetRecentCollectionItems();

            return (getItems && getRecent);
        }

        private async Task<bool> GetRecentCollectionItems()
        {
            var query = Utils.GetRecentItemsQuery(SelectedFolder.Id);

            try
            {
                Log.Info("Getting recent items for collection [{0}]", SelectedFolder.Name);

                var items = await ApiClient.GetItemsAsync(query);

                if (items != null && items.Items != null)
                {
                    var recent = await Utils.SortRecentItems(items.Items, App.SpecificSettings.IncludeTrailersInRecent);
                    recent.ForEach(item => RecentItems.Add(item));
                }

                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException(ex, "GetRecentCollectionItems()", NavigationService, Log);
                return false;
            }
        }

        private async Task<bool> GetItems()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = AuthenticationService.Current.LoggedInUserId,
                    SortBy = new[] {ItemSortBy.SortName},
                    SortOrder = SortOrder.Ascending,
                    Fields = new[]
                    {
                        ItemFields.SortName,
                        ItemFields.Genres,
                        ItemFields.ProviderIds,
                        ItemFields.ParentId,
                        ItemFields.CumulativeRunTimeTicks
                    },
                    ExcludeItemTypes = SelectedFolder != null && SelectedFolder.Name.ToLower().Contains("recent") ? new[] {"Season", "Series"} : null,
                    ImageTypeLimit = 1,
                    EnableImageTypes = new[] { ImageType.Backdrop, ImageType.Primary, }
                };
                var isRecent = false;
                if (SelectedPerson != null)
                {
                    Log.Info("Getting items for {0}", SelectedPerson.Name);
                    PageTitle = SelectedPerson.Name.ToLower();
                    query.Person = SelectedPerson.Name;
                    query.PersonTypes = new[] {SelectedPerson.Type};
                    query.Recursive = true;
                }
                else
                {
                    if (SelectedFolder.Name.Contains(AppResources.LabelRecent.ToLower()))
                    {
                        Log.Info("Getting recent items");
                        PageTitle = AppResources.LabelRecent.ToLower();
                        query = Utils.GetRecentItemsQuery(excludedItemTypes: new[] {"Photo", "Season", "Series"});
                        isRecent = true;
                    }
                    else if (SelectedFolder.Name.Equals(AppResources.Favourites.ToLower()))
                    {
                        Log.Info("Getting favourite items");
                        PageTitle = AppResources.Favourites.ToLower();
                        query.Filters = new[] {ItemFilter.IsFavorite};
                        query.Recursive = true;
                    }
                    else if (SelectedFolder.Type.StartsWith(AppResources.Genre))
                    {
                        Log.Info("Getting items for genre [{0}]", SelectedFolder.Name);
                        PageTitle = SelectedFolder.Name.ToLower();
                        query.Genres = new[] {SelectedFolder.Name};
                        query.Recursive = true;

                        if (SelectedFolder.Type.Contains(" - " + AppResources.LabelTv.ToUpper()))
                        {
                            query.IncludeItemTypes = new[] {"Series"};
                        }
                        else if (SelectedFolder.Type.Contains(" - " + AppResources.LabelMovies.ToUpper()))
                        {
                            query.ExcludeItemTypes = new[] { "Series" };
                            query.IncludeItemTypes = new[] {"Movie", "Trailer"};
                        }
                    }
                    else
                    {
                        Log.Info("Getting items for folder [{0}]", SelectedFolder.Name);
                        PageTitle = SelectedFolder.Name.ToLower();
                        query.ParentId = SelectedFolder.Id;
                    }
                }
                var items = await ApiClient.GetItemsAsync(query);
                
                CurrentItems = isRecent ? await Utils.SortRecentItems(items.Items, App.SpecificSettings.IncludeTrailersInRecent) : items.Items.ToList();
                return true;
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("GetItems()", ex, NavigationService, Log);

                App.ShowMessage(AppResources.ErrorGettingData);
                return false;
            }
        }

        private void SortList()
        {
            SetProgressBar(AppResources.SysTrayRegrouping);

            var emptyGroups = new List<Group<BaseItemDto>>();

            Log.Info("Sorting by [{0}]", GroupBy);

            switch (GroupBy)
            {
                case GroupBy.Name:
                    GroupHeaderTemplate = (DataTemplate) Application.Current.Resources["LLSGroupHeaderTemplateName"];
                    GroupItemTemplate = (Style) Application.Current.Resources["LLSGroupItemStyle"];
                    var headers = new List<string> {"#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
                    headers.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));
                    var groupedNameItems = (from c in CurrentItems
                        group c by Utils.GetSortByNameHeader(c)
                        into grp
                        orderby grp.Key
                        select new Group<BaseItemDto>(grp.Key, grp)).ToList();
                    FolderGroupings = groupedNameItems.ToList();
                    break;
                case GroupBy.ProductionYear:
                    GroupHeaderTemplate = (DataTemplate) Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (Style) Application.Current.Resources["LLSGroupItemStyle"];
                    var movieYears = (from y in CurrentItems
                        where y.ProductionYear != null
                        orderby y.ProductionYear
                        select y.ProductionYear.ToString()).Distinct().ToList();
                    movieYears.Insert(0, "?");
                    movieYears.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));

                    var groupedYearItems = from t in CurrentItems
                        group t by GetSortByProductionYearHeader(t)
                        into grp
                        orderby grp.Key
                        select new Group<BaseItemDto>(grp.Key, grp);
                    FolderGroupings = groupedYearItems.ToList();
                    break;
                case GroupBy.Genre:
                    GroupHeaderTemplate = (DataTemplate) Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (Style) Application.Current.Resources["LLSGroupItemLongStyle"];
                    var genres = (from t in CurrentItems
                        where t.Genres != null
                        from s in t.Genres
                        select s).Distinct().ToList();
                    genres.Insert(0, "none");
                    genres.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));

                    var groupedGenreItems = (from genre in genres
                        let films = (from f in CurrentItems
                            where CheckGenre(f)
                            where f.Genres.Contains(genre)
                            orderby Utils.GetSortByNameHeader(f)
                            select f).ToList()
                        select new Group<BaseItemDto>(genre, films)).ToList();
                    FolderGroupings = groupedGenreItems.OrderBy(x => x.Title).ToList();
                    break;

            }

            SetProgressBar();
        }

        private bool CheckGenre(BaseItemDto dtoBaseItem)
        {
            if (!dtoBaseItem.Genres.IsNullOrEmpty())
            {
                return true;
            }
            dtoBaseItem.Genres = new List<string> {AppResources.LabelNone};
            return true;
        }

        private string GetSortByProductionYearHeader(BaseItemDto dtoBaseItem)
        {
            return dtoBaseItem.ProductionYear == null ? "?" : dtoBaseItem.ProductionYear.ToString();
        }

        public bool CanPinCollection { get; set; }

        public string PageTitle { get; set; }
        public BaseItemDto SelectedFolder { get; set; }

        [UsedImplicitly]
        private void OnSelectedFolderChanged()
        {
            ServerIdItem = SelectedFolder;            
        }
        public BaseItemPerson SelectedPerson { get; set; }

        [UsedImplicitly]
        private void OnSelectedPersonChanged()
        {
            //ServerIdItem = SelectedPerson;
        }

        public List<Group<BaseItemDto>> FolderGroupings { get; set; }
        public List<BaseItemDto> CurrentItems { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> RandomItems { get; set; }

        public GroupBy GroupBy { get; set; }
        public DataTemplate GroupHeaderTemplate { get; set; }
        public Style GroupItemTemplate { get; set; }
        public ItemsPanelTemplate ItemsPanelTemplate { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand CollectionPageLoaded { get; set; }
        public RelayCommand SeeMoreCommand { get; set; }
        public RelayCommand<BaseItemDto> NavigateTo { get; set; }
    }
}