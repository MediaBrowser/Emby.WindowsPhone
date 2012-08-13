using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.Model.Entities;
using GalaSoft.MvvmLight.Messaging;
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
    public class MovieViewModel : ViewModelBase
    {
        private readonly INavigationService NavService;
        /// <summary>
        /// Initializes a new instance of the MovieViewModel class.
        /// </summary>
        public MovieViewModel(INavigationService navService)
        {
            NavService = navService;
            if(IsInDesignMode)
            {
                SelectedMovie = new ApiBaseItemWrapper<ApiBaseItem>
                {
                    Item = new ApiBaseItem
                    {
                        Id = new Guid("6536a66e10417d69105bae71d41a6e6f"),
                        Name = "Jurassic Park",
                        SortName = "Jurassic Park",
                        Overview = "Lots of dinosaurs eating people!"
                    }
                };
            }
            else
            {
                WireCommands();
                WireMessages();
            }
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if(m.Notification.Equals(Constants.ShowMovieMsg))
                {
                    SelectedMovie = (ApiBaseItemWrapper<ApiBaseItem>) m.Sender;
                    
                    var emptyGroups = new List<Group<PersonInfo>>();
                    var headers = new List<string> {"Actor", "Director", "Writer", "Producer"};
                    headers.ForEach(item => emptyGroups.Add(new Group<PersonInfo>(item, new List<PersonInfo>())));

                    var groupedPeople = (from p in SelectedMovie.Item.People
                                         group p by p.PersonType.ToString()
                                         into grp
                                         orderby grp.Key
                                         select new Group<PersonInfo>(grp.Key, grp)).ToList();

                    CastAndCrew = (from g in groupedPeople.Union(emptyGroups)
                                   orderby g.Title
                                   select g).ToList();
                }
                else if(m.Notification.Equals(Constants.ClearFilmAndTvMsg))
                {
                    SelectedMovie = null;
                }
            });
        }

        private void WireCommands()
        {
            NavigateTopage = new RelayCommand<ApiBaseItemWrapper<ApiBaseItem>>(NavService.NavigateTopage);
        }

        // UI properties
        public string ProgressText { get; set; }
        public bool ProgressIsVisible { get; set; }

        public ApiBaseItemWrapper<ApiBaseItem> SelectedMovie { get; set; }
        public List<Group<PersonInfo>> CastAndCrew { get; set; }

        public RelayCommand<ApiBaseItemWrapper<ApiBaseItem>> NavigateTopage { get; set; }
    }
}