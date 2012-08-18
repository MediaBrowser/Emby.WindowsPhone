using GalaSoft.MvvmLight;
using MediaBrowser.WindowsPhone.Model;
using Newtonsoft.Json;
using GalaSoft.MvvmLight.Command;
using SharpGIS;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using MediaBrowser.Model.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;

namespace MediaBrowser.WindowsPhone.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService NavService;
        private bool hasLoaded;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(INavigationService navService)
        {
            Folders = new ObservableCollection<DTOBaseItem>();
            RecentItems = new ObservableCollection<DTOBaseItem>();
            if (IsInDesignMode)
            {
                RecentItems.Add(new DTOBaseItem { Id = new Guid("2fc6f321b5f8bbe842fcd0eed089561d"), Name = "A Night To Remember" } );
            }
            else
            {
                NavService = navService;                
                WireCommands();
                App.Settings.HostName = "192.168.0.2"; App.Settings.PortNo = "8096";
                App.Settings.LoggedInUser = new User { Id = new Guid("5d1cf7fce25943b790d140095457a42b") };
                DummyFolder = new DTOBaseItem
                                  {
                                      Type = "folder",
                                      Name = "recent"
                                  };
            }
        }

        private void WireCommands()
        {
            PageLoaded = new RelayCommand(async () =>
            {
                if (NavService.IsNetworkAvailable && App.Settings.CheckHostAndPort() && !hasLoaded)
                {
                    ProgressIsVisible = true;
                    ProgressText = "Loading folders...";

                    bool folderLoaded = await GetFolders();

                    ProgressText = "Getting recent items...";

                    bool recentLoaded = await GetRecent();

                    hasLoaded = (folderLoaded && recentLoaded);
                    ProgressIsVisible = false;
                    hasLoaded = true;
                }
            });

            NavigateToPage = new RelayCommand<DTOBaseItem>(NavService.NavigateTopage);
        }

        private async Task<bool> GetRecent()
        {
            bool result = false;

            string recentUrl = App.Settings.ApiUrl + "itemlist?listtype=recentlyaddeditems&userid=" + App.Settings.LoggedInUser.Id;
            string recentjson = string.Empty;
            try
            {
                recentjson = await new GZipWebClient().DownloadStringTaskAsync(recentUrl);
            }
            catch (Exception ex)
            {
                App.ShowMessage("", "Error connecting to service");
            }
            if (!string.IsNullOrEmpty(recentjson))
            {
                var recent = JsonConvert.DeserializeObject<List<DTOBaseItem>>(recentjson);
                RecentItems.Clear();
                recent.OrderBy(x => x.DateCreated).Take(6).ToList().ForEach(recentItem => RecentItems.Add(recentItem));
                result = true;
            }

            return result;
        }

        private async Task<bool> GetFolders()
        {
            bool result = false;

            string url = App.Settings.ApiUrl + "item?userid=" + App.Settings.LoggedInUser.Id;
            string folderjson = string.Empty;
            try
            {
                folderjson = await new GZipWebClient().DownloadStringTaskAsync(url);
            }
            catch (Exception exe)
            {
                App.ShowMessage("", "Error connecting to service1");
            }

            if (!string.IsNullOrEmpty(folderjson))
            {
                var item = JsonConvert.DeserializeObject<DTOBaseItem>(folderjson);
                Folders.Clear();
                item.Children.ToList().ForEach(folder => Folders.Add(folder));
                result = true;
            }

            return result;
        }

        // UI properties
        public bool ProgressIsVisible { get; set; }
        public string ProgressText { get; set; }

        public RelayCommand PageLoaded { get; set; }
        public RelayCommand<DTOBaseItem> NavigateToPage { get; set; }
        public ObservableCollection<DTOBaseItem> Folders { get; set; }
        public ObservableCollection<DTOBaseItem> RecentItems { get; set; }
        public DTOBaseItem DummyFolder { get; set; }
    }
}