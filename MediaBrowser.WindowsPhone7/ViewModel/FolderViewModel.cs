using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MediaBrowser.WindowsPhone.Resources;
using Microsoft.Phone.Shell;

#if !WP8
using ScottIsAFool.WindowsPhone;
#endif

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
        private readonly INavigationService NavService;
        private readonly ExtendedApiClient ApiClient;
        private bool dataLoaded;
        /// <summary>
        /// Initializes a new instance of the FolderViewModel class.
        /// </summary>
        public FolderViewModel(INavigationService navService, ExtendedApiClient apiClient)
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
                                        People = new []
                                                     {
                                                         new BaseItemPerson{Name = "Steven Spielberg", Type = "Director"},
                                                         new BaseItemPerson{Name = "Sam Neill", Type = "Actor"},
                                                         new BaseItemPerson{Name = "Richard Attenborough", Type = "Actor"},
                                                         new BaseItemPerson{Name = "Laura Dern", Type = "Actor"}
                                                     }

                                    });
            }
            else
            {
                ApiClient = apiClient;
                NavService = navService;
                WireCommands();
                WireMessages();
                SortBy = "name";
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.ShowFolderMsg))
                {
                    SelectedFolder = m.Sender as BaseItemDto;
                    dataLoaded = false;
                }
                else if (m.Notification.Equals(Constants.ChangeGroupingMsg))
                {
                    SortBy = (string)m.Sender;
                    SortList();
                }
                else if (m.Notification.Equals(Constants.ClearFoldersMsg))
                {
                    CurrentItems = null;
                    FolderGroupings = null;
                }
                else if (m.Notification.Equals(Constants.CollectionPinnedMsg))
                {
                    var tileUrl = (string) m.Sender;
                    var tile = ShellTile.ActiveTiles.SingleOrDefault(x => x.NavigationUri.ToString() == tileUrl);
                    CanPinCollection = tile == default(ShellTile);
                }
            });
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !dataLoaded)
                {
                    ProgressIsVisible = true;
                    ProgressText = AppResources.SysTrayGettingItems;

                    dataLoaded = await GetItems();

                    SortList();
                    ProgressIsVisible = false;
                }
            });

            CollectionPageLoaded = new RelayCommand(async () =>
                                                        {
                                                            if (NavService.IsNetworkAvailable && !dataLoaded && SelectedFolder != null)
                                                            {
                                                                ProgressText = AppResources.SysTrayCheckingCollection;
                                                                ProgressIsVisible = true;

                                                                var tileUrl = string.Format(Constants.PhoneCollectionTileUrlFormat, SelectedFolder.Id, SelectedFolder.Name);
                                                                var shellExists = ShellTile.ActiveTiles.SingleOrDefault(x => x.NavigationUri.ToString() == tileUrl);

                                                                CanPinCollection = shellExists == default(ShellTile);

                                                                dataLoaded = await GetCollectionItems();

                                                                ProgressText = string.Empty;
                                                                ProgressIsVisible = false;
                                                            }

                                                            if (CurrentItems != null && CurrentItems.Any())
                                                            {
                                                                GetRandomItems();
                                                            }
                                                        });

            NavigateToPage = new RelayCommand<BaseItemDto>(NavService.NavigateToPage);
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
            var query = new ItemQuery
            {
                Filters = new[] { ItemFilter.IsRecentlyAdded, ItemFilter.IsNotFolder, },
                UserId = App.Settings.LoggedInUser.Id,
                Fields = new[]
                                                 {
                                                     ItemFields.SeriesInfo,
                                                     ItemFields.DateCreated,
                                                     ItemFields.UserData, 
                                                 },
                ParentId = SelectedFolder.Id,
                Recursive = true
            };

            try
            {
                var items = await ApiClient.GetItemsAsync(query);

                if (items != null && items.Items != null)
                {
                    var recent = await Utils.SortRecentItems(items.Items);
                    recent.ForEach(item => RecentItems.Add(item));
                }

                return true;
            }
            catch 
            {
                return false;
            }
        }

        private async Task<bool> GetItems()
        {
            try
            {
                var query = new ItemQuery
                {
                    UserId = App.Settings.LoggedInUser.Id,
                    SortBy = new[] { ItemSortBy.SortName },
                    SortOrder = SortOrder.Ascending,
                    Fields = new[] { ItemFields.SortName, ItemFields.UserData, ItemFields.Genres, }
                };
                if (SelectedPerson != null)
                {
                    PageTitle = SelectedPerson.Name.ToLower();
                    query.Person = SelectedPerson.Name;
                    query.PersonType = SelectedPerson.Type;
                    query.Recursive = true;
                }
                else
                {
                    if (SelectedFolder.Name.Contains("recent"))
                    {
                        PageTitle = AppResources.Recent.ToLower();
                        query.Filters = new[] { ItemFilter.IsRecentlyAdded };
                        query.Recursive = true;
                    }
                    else if (SelectedFolder.Type.Equals("Genre"))
                    {
                        PageTitle = SelectedFolder.Type.ToLower();
                        query.Genres = new[] { SelectedFolder.Name };
                        query.Recursive = true;
                    }
                    else
                    {
                        PageTitle = SelectedFolder.Name.ToLower();
                        query.ParentId = SelectedFolder.Id;
                    }
                }
                var items = await ApiClient.GetItemsAsync(query);
                CurrentItems = items.Items.ToList();
                return true;
            }
            catch
            {
                App.ShowMessage("", AppResources.ErrorGettingData);
                return false;
            }
        }

        private void SortList()
        {
            ProgressText = AppResources.SysTrayRegrouping;
            ProgressIsVisible = true;
            var emptyGroups = new List<Group<BaseItemDto>>();
            switch (SortBy)
            {
                case "name":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateName"];
#if WP8
                    GroupItemTemplate = (Style)Application.Current.Resources["LLSGroupItemStyle"];
#else
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["WrapPanelTemplate"];
#endif
                    var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                    headers.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item, new List<BaseItemDto>())));
                    var groupedNameItems = (from c in CurrentItems
                                            group c by Utils.GetSortByNameHeader(c)
                                                into grp
                                                orderby grp.Key
                                                select new Group<BaseItemDto>(grp.Key, grp)).ToList();
#if WP8
                    FolderGroupings = groupedNameItems.ToList();
#else
                    FolderGroupings = (from g in groupedNameItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
#endif
                    break;
                case "production year":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
#if WP8
                    GroupItemTemplate = (Style)Application.Current.Resources["LLSGroupItemStyle"];
#else
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["WrapPanelTemplate"];
#endif
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
#if WP8
                    FolderGroupings = groupedYearItems.ToList();
#else
                    FolderGroupings = (from g in groupedYearItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
#endif
                    break;
                case "genre":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
#if WP8
                    GroupItemTemplate = (Style)Application.Current.Resources["LLSGroupItemLongStyle"];
#else
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
#endif
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
#if WP8
                    FolderGroupings = groupedGenreItems.ToList();
#else
                    FolderGroupings = (from g in groupedGenreItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
#endif
                    break;
//                case "studio":
//                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
//#if WP8
//                    GroupItemTemplate = (Style)Application.Current.Resources["LLSGroupItemLongStyle"];
//#else
//                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
//                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
//#endif
//                    var studios = (from s in CurrentItems
//                                   where s.Studios != null
//                                   from st in s.Studios
//                                   select st).Distinct().ToList();
//                    studios.Insert(0, new BaseItemStudio { Name = "none" });
//                    studios.ForEach(item => emptyGroups.Add(new Group<BaseItemDto>(item.Name, new List<BaseItemDto>())));

//                    var groupedStudioItems = (from studio in studios
//                                              let films = (from f in CurrentItems
//                                                           where CheckStudio(f)
//                                                           where f.Studios.Contains(studio)
//                                                           orderby GetSortByNameHeader(f)
//                                                           select f).ToList()
//                                              select new Group<BaseItemDto>(studio.Name, films)).ToList();
//#if WP8
//                    FolderGroupings = groupedStudioItems.ToList();
//#else
//                    FolderGroupings = (from g in groupedStudioItems.Union(emptyGroups)
//                                       orderby g.Title
//                                       select g).ToList();
//#endif
//                    break;
            }
            ProgressIsVisible = false;
        }

        //private bool CheckStudio(BaseItemDto dtoBaseItem)
        //{
        //    if (dtoBaseItem.Studios != null && dtoBaseItem.Studios.Any())
        //    {
        //        return true;
        //    }
        //    dtoBaseItem.Studios = new[] { new BaseItemStudio { Name = "none" } };
        //    return true;
        //}

        private bool CheckGenre(BaseItemDto dtoBaseItem)
        {
            if (dtoBaseItem.Genres != null && dtoBaseItem.Genres.Any())
            {
                return true;
            }
            dtoBaseItem.Genres = new List<string> { "none" };
            return true;
        }

        private string GetSortByProductionYearHeader(BaseItemDto dtoBaseItem)
        {
            return dtoBaseItem.ProductionYear == null ? "?" : dtoBaseItem.ProductionYear.ToString();
        }

        // Shell properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }
        public bool CanPinCollection { get; set; }

        public string PageTitle { get; set; }
        public BaseItemDto SelectedFolder { get; set; }
        public BaseItemPerson SelectedPerson { get; set; }
        public List<Group<BaseItemDto>> FolderGroupings { get; set; }
        public List<BaseItemDto> CurrentItems { get; set; }
        public ObservableCollection<BaseItemDto> RecentItems { get; set; }
        public ObservableCollection<BaseItemDto> RandomItems { get; set; }

        public string SortBy { get; set; }
        public DataTemplate GroupHeaderTemplate { get; set; }
#if WP8
        public Style GroupItemTemplate { get; set; }
#else
        public DataTemplate GroupItemTemplate { get; set; }
#endif
        public ItemsPanelTemplate ItemsPanelTemplate { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand CollectionPageLoaded { get; set; }
        public RelayCommand<BaseItemDto> NavigateToPage { get; set; }
    }
}