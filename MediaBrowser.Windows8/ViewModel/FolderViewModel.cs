using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Querying;
using MediaBrowser.Windows8.Model;
using Windows.UI.Xaml;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.Windows8.ViewModel
{
    public class FolderViewModel : ViewModelBase
    {
        private readonly NavigationService navigtaionService;
        private readonly ExtendedApiClient ApiClient;
        private bool collectionLoaded;
        private BaseItemDto[] children;

        public FolderViewModel(NavigationService navigation, ExtendedApiClient apiClient)
        {
            navigtaionService = navigation;
            ApiClient = apiClient;
            if (IsInDesignMode)
            {
                SelectedCollection = new BaseItemDto
                                         {
                                             Name = "Movies"
                                         };
                children = new[]
                               {
                                   new BaseItemDto
                                       {
                                           Id = "969072baf139763483c275b34d69e8c4",
                                           Name = "Jurassic Park",
                                           SortName = "Jurassic Park",
                                           Overview = "Lots of dinosaurs eating everyone!! Oh noes",
                                           Path = @"g:\jurassic park.mkv",
                                           AspectRatio = "16:9",
                                           ProductionYear = 1993,
                                           RunTimeTicks = 67857820000,
                                           CommunityRating = (float) 8.9,
                                           Genres = new[] {"Adventure", "Family", "Sci-Fi", "Science Fiction"}.ToList(),
                                           UserData = new UserItemDataDto
                                                          {
                                                              Likes = false
                                                          }
                                       },
                                   new BaseItemDto
                                       {
                                           Id = "0d34ace1bceb5a94aba168a18cca4b58",
                                           Name = "The Lost World: Jurassic Park",
                                           SortName = "Jurassic Park III",
                                           Overview = "Lots of dinosaurs eating everyone!! Oh noes",
                                           Path = @"g:\jurassic park.mkv",
                                           AspectRatio = "16:9",
                                           ProductionYear = 1997,
                                           RunTimeTicks = 67857820000,
                                           CommunityRating = (float) 6.7,
                                           Genres = new[] {"Adventure", "Family", "Sci-Fi", "Science Fiction"}.ToList(),
                                           UserData = new UserItemDataDto
                                                          {
                                                              Likes = true,
                                                              IsFavorite= true
                                                          }
                                       },
                                       new BaseItemDto
                                           {
                                               Name= "Goldeneye",
                                               SortName= "James Bond 17 Goldeneye",
                                               Id= "5662bce16e94dea7a52ced2ec7b40791",
                                               Genres = new [] { "Action","Adventure","Thriller","Foreign"}.ToList(),
                                               CommunityRating= (float)7.7,
                                               RunTimeTicks = 78003760000,
                                               ProductionYear= 1995,
                                               UserData = new UserItemDataDto
                                                              {
                                                                  IsFavorite = true
                                                              }
                                           }
                               };
                SortItems();
                
            }
            else
            {
                WireCommands();
                WireMessages();
            }
        }

        private void WireCommands()
        {
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, async m =>
            {
                if (m.Notification.Equals(Constants.ShowFolderMsg))
                {
                    SelectedCollection = (BaseItemDto)m.Sender;
                    collectionLoaded = false;
                }
                else if (m.Notification.Equals(Constants.FolderViewLoadedMsg))
                {
                    if (SelectedCollection != null)
                        PageTitle = SelectedCollection.Name;
                    else if (SelectedPerson != null)
                        PageTitle = SelectedPerson.Name;
                    if (navigtaionService.IsNetworkAvailable && !collectionLoaded)
                    {
                        ProgressVisibility = Visibility.Visible;
                        ProgressText = "Getting items...";
                        try
                        {
                            var items = new ItemsResult();
                            var query = new ItemQuery
                            {
                                UserId = App.Settings.LoggedInUser.Id,
                                SortBy = new []{ ItemSortBy.SortName },
                                SortOrder = SortOrder.Ascending,
                                Fields = new[] { ItemFields.SortName, ItemFields.UserData, }
                            };

                            if (SelectedPerson != null)
                            {
                                query.Person = SelectedPerson.Name;
                                query.PersonType = SelectedPerson.Type;
                                query.Recursive = true;
                            }
                            else if (SelectedCollection != null)
                            {
                                if (SelectedCollection.Type.Equals("genre"))
                                {
                                    query.Genres = new[] { SelectedCollection.Name };
                                    query.Recursive = true;
                                }
                                else
                                {
                                    query.ParentId = SelectedCollection.Id;
                                }
                            }

                            items = await ApiClient.GetItemsAsync(query);

                            children = items.Items;
                            ProgressText = "Grouping items...";
                            await SortItems();
                            collectionLoaded = true;
                        }
                        catch
                        {
                        }
                        ProgressVisibility = Visibility.Collapsed;
                    }
                }
                else if (m.Notification.Equals(Constants.ClearEverythingMsg))
                {
                    Reset();
                }
            });
        }

        private void Reset()
        {
            SelectedCollection = null;
            GroupedMovies = null;
        }

        private async Task SortItems()
        {
            var groups = new ObservableCollection<GroupInfoList<object>>();
            if (children != null)
            {
                await Task.Run(() =>
                                   {
                                       foreach(var i in children) if (i.UserData == null) i.UserData = new UserItemDataDto();
                                       var query = from item in children.OrderBy(x => x.SortName).ToList()
                                                   group item by GetSortByNameHeader(item)
                                                       into grp
                                                       orderby grp.Key
                                                       select new { GroupName = grp.Key, Items = grp };
                                       foreach (var g in query)
                                       {
                                           var info = new GroupInfoList<object> { Key = g.GroupName };
                                           info.AddRange(g.Items);
                                           groups.Add(info);
                                       }

                                   });
            }
            ItemCount = string.Format("{0} items", children.Count());
            GroupedMovies = groups;
        }

        private string GetSortByNameHeader(BaseItemDto dtoBaseItem)
        {
            string name = !string.IsNullOrEmpty(dtoBaseItem.SortName) ? dtoBaseItem.SortName : dtoBaseItem.Name;
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

        public string ProgressText { get; set; }
        public Visibility ProgressVisibility { get; set; }
        public bool CanPinCollection { get; set; }

        public BaseItemDto SelectedCollection { get; set; }
        public BaseItemPerson SelectedPerson { get; set; }
        public string PageTitle { get; set; }
        public string ItemCount { get; set; }
        public ObservableCollection<GroupInfoList<object>> GroupedMovies { get; set; }
    }
}
