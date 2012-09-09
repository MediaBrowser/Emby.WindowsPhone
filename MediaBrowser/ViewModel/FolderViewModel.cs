using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using MediaBrowser.ApiInteraction.WindowsPhone;
using MediaBrowser.WindowsPhone.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.DTO;
using ScottIsAFool.WindowsPhone;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
        private readonly ApiClient ApiClient;
        private bool dataLoaded;
        /// <summary>
        /// Initializes a new instance of the FolderViewModel class.
        /// </summary>
        public FolderViewModel(INavigationService navService, ApiClient apiClient)
        {
            if (!IsInDesignMode)
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
                    SelectedFolder = m.Sender as DTOBaseItem;
                    dataLoaded = false;
                }
                else if(m.Notification.Equals(Constants.ChangeGroupingMsg))
                {
                    SortBy = (string) m.Sender;
                    SortList();
                }
                else if(m.Notification.Equals(Constants.ClearFoldersMsg))
                {
                    CurrentItems = null;
                    FolderGroupings = null;
                }
            });
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !dataLoaded)
                {
                    if (SelectedFolder != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting items...";

                        dataLoaded = await GetItems();

                        SortList();
                        ProgressIsVisible = false;
                    }
                }
            });

            NavigateToPage = new RelayCommand<DTOBaseItem>(NavService.NavigateTopage);
        }

        private async Task<bool> GetItems()
        {
            try
            {
                if (SelectedFolder.Name.Contains("recent"))
                {
                    var items = await ApiClient.GetRecentlyAddedItemsAsync(App.Settings.LoggedInUser.Id);
                    CurrentItems = items.ToList();
                }
                else
                {
                    var items = await ApiClient.GetItemAsync(SelectedFolder.Id, App.Settings.LoggedInUser.Id);
                    CurrentItems = items.Children.ToList();
                }
                return true;
            }
            catch
            {
                App.ShowMessage("", "Error getting data");
                return false;
            }
        }

        private void SortList()
        {
            ProgressText = "Re-grouping...";
            ProgressIsVisible = true;
            var emptyGroups = new List<Group<DTOBaseItem>>();
            switch (SortBy)
            {
                case "name":
                    GroupHeaderTemplate = (DataTemplate) Application.Current.Resources["LLSGroupHeaderTemplateName"];
                    GroupItemTemplate = (DataTemplate) Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate) Application.Current.Resources["WrapPanelTemplate"];
                    var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                    headers.ForEach(item => emptyGroups.Add(new Group<DTOBaseItem>(item, new List<DTOBaseItem>())));
                    var groupedNameItems = (from c in CurrentItems
                                            group c by GetSortByNameHeader(c)
                                                into grp
                                                orderby grp.Key
                                                select new Group<DTOBaseItem>(grp.Key, grp)).ToList();
                    FolderGroupings = (from g in groupedNameItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "production year":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["WrapPanelTemplate"];
                    var movieYears = (from y in CurrentItems
                                      where y.ProductionYear != null
                                      orderby y.ProductionYear
                                      select y.ProductionYear.ToString()).Distinct().ToList();
                    movieYears.Insert(0, "?");
                    movieYears.ForEach(item => emptyGroups.Add(new Group<DTOBaseItem>(item, new List<DTOBaseItem>())));

                    var groupedYearItems = from t in CurrentItems
                                           group t by GetSortByProductionYearHeader(t)
                                               into grp
                                               orderby grp.Key
                                               select new Group<DTOBaseItem>(grp.Key, grp);
                    FolderGroupings = (from g in groupedYearItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "genre":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
                    var genres = (from t in CurrentItems
                                  where t.Genres != null
                                      from s in t.Genres
                                      select s).Distinct().ToList();
                    genres.Insert(0, "none");
                    genres.ForEach(item => emptyGroups.Add(new Group<DTOBaseItem>(item, new List<DTOBaseItem>())));

                    var groupedGenreItems = (from genre in genres
                                let films = (from f in CurrentItems
                                             where CheckGenre(f)
                                             where f.Genres.Contains(genre)
                                             orderby GetSortByNameHeader(f)
                                             select f).ToList()
                                select new Group<DTOBaseItem>(genre, films)).ToList();

                    FolderGroupings = (from g in groupedGenreItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "studio":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
                    var studios = (from s in CurrentItems
                                   where s.Studios != null
                                   from st in s.Studios
                                   select st).Distinct().ToList();
                    studios.Insert(0, new BaseItemStudio {Name = "none"});
                    studios.ForEach(item => emptyGroups.Add(new Group<DTOBaseItem>(item.Name, new List<DTOBaseItem>())));

                    var groupedStudioItems = (from studio in studios
                                              let films = (from f in CurrentItems
                                                           where CheckStudio(f)
                                                           where f.Studios.Contains(studio)
                                                           orderby GetSortByNameHeader(f)
                                                           select f).ToList()
                                              select new Group<DTOBaseItem>(studio.Name, films)).ToList();
                    FolderGroupings = (from g in groupedStudioItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
            }
            ProgressIsVisible = false;
        }

        private bool CheckStudio(DTOBaseItem DTOBaseItem)
        {
            if (DTOBaseItem.Studios != null && DTOBaseItem.Studios.Any())
            {
                return true;
            }
            DTOBaseItem.Studios = new[] {new BaseItemStudio {Name = "none"}};
            return true;
        }

        private bool CheckGenre(DTOBaseItem DTOBaseItem)
        {
            if (DTOBaseItem.Genres != null && DTOBaseItem.Genres.Any())
            {
                return true;
            }
            DTOBaseItem.Genres = new string[] { "none" };
            return true;
        }

        private string GetSortByProductionYearHeader(DTOBaseItem DTOBaseItem)
        {
            return DTOBaseItem.ProductionYear == null ? "?" : DTOBaseItem.ProductionYear.ToString();
        }

        private string GetSortByNameHeader(DTOBaseItem DTOBaseItem)
        {
            string name = !string.IsNullOrEmpty(DTOBaseItem.SortName) ? DTOBaseItem.SortName : DTOBaseItem.Name;
            string[] words = name.Split(' ');
            char l = name.ToLower()[0];
            if (words[0].ToLower().Equals("the") ||
                words[0].ToLower().Equals("a") ||
                words[0].ToLower().Equals("an"))
            {
                if (words.Length > 0)
                    l = words[1].ToLower()[0];
            }
            if (l >= 'a' && l <= 'z')
            {
                return l.ToString();
            }
            return '#'.ToString();
        }

        // Shell properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public DTOBaseItem SelectedFolder { get; set; }
        public List<Group<DTOBaseItem>> FolderGroupings { get; set; }
        public List<DTOBaseItem> CurrentItems { get; set; }

        public string SortBy { get; set; }
        public DataTemplate GroupHeaderTemplate { get; set; }
        public DataTemplate GroupItemTemplate { get; set; }
        public ItemsPanelTemplate ItemsPanelTemplate { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand<DTOBaseItem> NavigateToPage { get; set; }
    }
}