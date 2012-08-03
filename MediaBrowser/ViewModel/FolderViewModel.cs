using GalaSoft.MvvmLight;
using MediaBrowser.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using ScottIsAFool.WindowsPhone;
using System.Collections.Generic;
using SharpGIS;
using Newtonsoft.Json;
using System.Linq;
using System.Windows;

namespace MediaBrowser.ViewModel
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
        /// <summary>
        /// Initializes a new instance of the FolderViewModel class.
        /// </summary>
        public FolderViewModel(INavigationService navService)
        {
            if (IsInDesignMode)
            {

            }
            else
            {
                NavService = navService;
                WireCommands();
                WireMessages();
                SortBy = "studio";
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.ShowFolderMsg))
                {
                    SelectedFolder = m.Sender as ApiBaseItem;
                    FolderGroupings = null;
                }
            });
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort())
                {
                    if (SelectedFolder != null)
                    {
                        ProgressIsVisible = true;
                        ProgressText = "Getting folder items...";
                        string url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}", App.Settings.LoggedInUser.Id, SelectedFolder.Id);
                        string folderJson;
                        try
                        {
                            folderJson = await new GZipWebClient().DownloadStringTaskAsync(url);
                        }
                        catch
                        {
                            App.ShowMessage("", "Error downloading folder information");
                            return;
                        }
                        var folder = JsonConvert.DeserializeObject<ApiBaseItemWrapper<ApiBaseItem>>(folderJson);
                        CurrentItems = folder.Children.ToList();
                        SortList(CurrentItems);
                        ProgressIsVisible = false;
                    }
                }
            });            
        }

        private void SortList(List<ApiBaseItemWrapper<ApiBaseItem>> children)
        {
            var emptyGroups = new List<Group<ApiBaseItem>>();
            switch (SortBy)
            {
                case "name":
                    GroupHeaderTemplate = (DataTemplate)App.Current.Resources["LLSGroupHeaderTemplateName"];
                    var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                    headers.ForEach(item => emptyGroups.Add(new Group<ApiBaseItem>(item, new List<ApiBaseItem>())));
                    var groupedNameItems = (from c in children
                                            group c.Item by GetSortByNameHeader(c.Item)
                                                into grp
                                                orderby grp.Key
                                                select new Group<ApiBaseItem>(grp.Key, grp)).ToList();
                    FolderGroupings = (from g in groupedNameItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "production year":
                    GroupHeaderTemplate = (DataTemplate)App.Current.Resources["LLSGroupHeaderTemplateLong"];
                    var movieYears = (from y in children
                                      where y.Item.ProductionYear != null
                                      orderby y.Item.ProductionYear
                                      select y.Item.ProductionYear.ToString()).Distinct().ToList();
                    movieYears.Insert(0, "?");
                    movieYears.ForEach(item => emptyGroups.Add(new Group<ApiBaseItem>(item, new List<ApiBaseItem>())));

                    var groupedYearItems = from t in children
                                           group t.Item by GetSortByProductionYearHeader(t.Item)
                                               into grp
                                               orderby grp.Key
                                               select new Group<ApiBaseItem>(grp.Key, grp);
                    FolderGroupings = (from g in groupedYearItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "genre":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    var genres = (from t in children
                                  where t.Item.Genres != null
                                      from s in t.Item.Genres
                                      select s).Distinct().ToList();
                    genres.Insert(0, "none");
                    genres.ForEach(item => emptyGroups.Add(new Group<ApiBaseItem>(item, new List<ApiBaseItem>())));

                    var groupedGenreItems = (from genre in genres
                                let films = (from f in children
                                             where CheckGenre(f.Item)
                                             where f.Item.Genres.Contains(genre)
                                             orderby GetSortByNameHeader(f.Item)
                                             select f.Item).ToList()
                                select new Group<ApiBaseItem>(genre, films)).ToList();

                    FolderGroupings = (from g in groupedGenreItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "studio":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    var studios = (from s in children
                                   where s.Item.Studios != null
                                   from st in s.Item.Studios
                                   select st).Distinct().ToList();
                    studios.Insert(0, "none");
                    studios.ForEach(item => emptyGroups.Add(new Group<ApiBaseItem>(item, new List<ApiBaseItem>())));

                    var groupedStudioItems = (from studio in studios
                                              let films = (from f in children
                                                           where CheckStudio(f.Item)
                                                           where f.Item.Studios.Contains(studio)
                                                           orderby GetSortByNameHeader(f.Item)
                                                           select f.Item).ToList()
                                              select new Group<ApiBaseItem>(studio, films)).ToList();
                    FolderGroupings = (from g in groupedStudioItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
            }
        }

        private bool CheckStudio(ApiBaseItem apiBaseItem)
        {
            if (apiBaseItem.Studios != null && apiBaseItem.Studios.Any())
            {
                return true;
            }
            apiBaseItem.Studios = new List<string> { "none" };
            return true;
        }

        private bool CheckGenre(ApiBaseItem apiBaseItem)
        {
            if(apiBaseItem.Genres != null && apiBaseItem.Genres.Any())
            {
                return true;
            }
            apiBaseItem.Genres = new List<string> {"none"};
            return true;
        }

        private string GetSortByProductionYearHeader(ApiBaseItem apiBaseItem)
        {
            return apiBaseItem.ProductionYear == null ? "?" : apiBaseItem.ProductionYear.ToString();
        }

        private string GetSortByNameHeader(ApiBaseItem apiBaseItem)
        {
            string name = "";
            name = !string.IsNullOrEmpty(apiBaseItem.SortName) ? apiBaseItem.SortName : apiBaseItem.Name;
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

        public ApiBaseItem SelectedFolder { get; set; }
        public List<Group<ApiBaseItem>> FolderGroupings { get; set; }
        public List<ApiBaseItemWrapper<ApiBaseItem>> CurrentItems { get; set; }

        public string SortBy { get; set; }
        public DataTemplate GroupHeaderTemplate { get; set; }
        public DataTemplate GroupItemTemplate { get; set; }

        public RelayCommand PageLoaded { get; set; }
    }
}