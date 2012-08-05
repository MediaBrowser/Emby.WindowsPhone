using System.Windows.Controls;
using GalaSoft.MvvmLight;
using MediaBrowser.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using Microsoft.Phone.Controls;
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
        private bool dataLoaded;
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
                SortBy = "name";
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.ShowFolderMsg))
                {
                    SelectedFolder = m.Sender as ApiBaseItem;
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
                        string url;
                        if(SelectedFolder.Name.Equals("recent"))
                        {
                            url = string.Format(App.Settings.ApiUrl + "recentlyaddeditems?userid={0}",
                                                App.Settings.LoggedInUser.Id);
                        }
                        else
                        {
                            url = string.Format(App.Settings.ApiUrl + "item?userid={0}&id={1}",
                                                App.Settings.LoggedInUser.Id, SelectedFolder.Id);
                        }
                        string folderJson;
                        try
                        {
                            folderJson = await new GZipWebClient().DownloadStringTaskAsync(url);
                        }
                        catch
                        {
                            App.ShowMessage("", "Error downloading information");
                            return;
                        }
                        if(SelectedFolder.Name.Equals("recent"))
                        {
                            var folder = JsonConvert.DeserializeObject<List<ApiBaseItemWrapper<ApiBaseItem>>>(folderJson);
                            CurrentItems = folder;
                        }
                        else
                        {
                            var folder = JsonConvert.DeserializeObject<ApiBaseItemWrapper<ApiBaseItem>>(folderJson);
                            CurrentItems = folder.Children.ToList();                            
                        }
                        SortList();
                        ProgressIsVisible = false;
                        dataLoaded = true;
                    }
                }
            });

            NavigateToPage = new RelayCommand<ApiBaseItemWrapper<ApiBaseItem>>(NavService.NavigateTopage);
        }

        private void SortList()
        {
            ProgressText = "Re-grouping...";
            ProgressIsVisible = true;
            var emptyGroups = new List<Group<ApiBaseItemWrapper<ApiBaseItem>>>();
            switch (SortBy)
            {
                case "name":
                    GroupHeaderTemplate = (DataTemplate) Application.Current.Resources["LLSGroupHeaderTemplateName"];
                    GroupItemTemplate = (DataTemplate) Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate) Application.Current.Resources["WrapPanelTemplate"];
                    var headers = new List<string> { "#", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                    headers.ForEach(item => emptyGroups.Add(new Group<ApiBaseItemWrapper<ApiBaseItem>>(item, new List<ApiBaseItemWrapper<ApiBaseItem>>())));
                    var groupedNameItems = (from c in CurrentItems
                                            group c by GetSortByNameHeader(c.Item)
                                                into grp
                                                orderby grp.Key
                                                select new Group<ApiBaseItemWrapper<ApiBaseItem>>(grp.Key, grp)).ToList();
                    FolderGroupings = (from g in groupedNameItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "production year":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplate"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["WrapPanelTemplate"];
                    var movieYears = (from y in CurrentItems
                                      where y.Item.ProductionYear != null
                                      orderby y.Item.ProductionYear
                                      select y.Item.ProductionYear.ToString()).Distinct().ToList();
                    movieYears.Insert(0, "?");
                    movieYears.ForEach(item => emptyGroups.Add(new Group<ApiBaseItemWrapper<ApiBaseItem>>(item, new List<ApiBaseItemWrapper<ApiBaseItem>>())));

                    var groupedYearItems = from t in CurrentItems
                                           group t by GetSortByProductionYearHeader(t.Item)
                                               into grp
                                               orderby grp.Key
                                               select new Group<ApiBaseItemWrapper<ApiBaseItem>>(grp.Key, grp);
                    FolderGroupings = (from g in groupedYearItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "genre":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
                    var genres = (from t in CurrentItems
                                  where t.Item.Genres != null
                                      from s in t.Item.Genres
                                      select s).Distinct().ToList();
                    genres.Insert(0, "none");
                    genres.ForEach(item => emptyGroups.Add(new Group<ApiBaseItemWrapper<ApiBaseItem>>(item, new List<ApiBaseItemWrapper<ApiBaseItem>>())));

                    var groupedGenreItems = (from genre in genres
                                let films = (from f in CurrentItems
                                             where CheckGenre(f.Item)
                                             where f.Item.Genres.Contains(genre)
                                             orderby GetSortByNameHeader(f.Item)
                                             select f).ToList()
                                select new Group<ApiBaseItemWrapper<ApiBaseItem>>(genre, films)).ToList();

                    FolderGroupings = (from g in groupedGenreItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
                case "studio":
                    GroupHeaderTemplate = (DataTemplate)Application.Current.Resources["LLSGroupHeaderTemplateLong"];
                    GroupItemTemplate = (DataTemplate)Application.Current.Resources["LLSGroupItemTemplateLong"];
                    ItemsPanelTemplate = (ItemsPanelTemplate)Application.Current.Resources["StackPanelVerticalTemplate"];
                    var studios = (from s in CurrentItems
                                   where s.Item.Studios != null
                                   from st in s.Item.Studios
                                   select st).Distinct().ToList();
                    studios.Insert(0, "none");
                    studios.ForEach(item => emptyGroups.Add(new Group<ApiBaseItemWrapper<ApiBaseItem>>(item, new List<ApiBaseItemWrapper<ApiBaseItem>>())));

                    var groupedStudioItems = (from studio in studios
                                              let films = (from f in CurrentItems
                                                           where CheckStudio(f.Item)
                                                           where f.Item.Studios.Contains(studio)
                                                           orderby GetSortByNameHeader(f.Item)
                                                           select f).ToList()
                                              select new Group<ApiBaseItemWrapper<ApiBaseItem>>(studio, films)).ToList();
                    FolderGroupings = (from g in groupedStudioItems.Union(emptyGroups)
                                       orderby g.Title
                                       select g).ToList();
                    break;
            }
            ProgressIsVisible = false;
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
            string name = !string.IsNullOrEmpty(apiBaseItem.SortName) ? apiBaseItem.SortName : apiBaseItem.Name;
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
        public List<Group<ApiBaseItemWrapper<ApiBaseItem>>> FolderGroupings { get; set; }
        public List<ApiBaseItemWrapper<ApiBaseItem>> CurrentItems { get; set; }

        public string SortBy { get; set; }
        public DataTemplate GroupHeaderTemplate { get; set; }
        public DataTemplate GroupItemTemplate { get; set; }
        public ItemsPanelTemplate ItemsPanelTemplate { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand<ApiBaseItemWrapper<ApiBaseItem>> NavigateToPage { get; set; }
    }
}