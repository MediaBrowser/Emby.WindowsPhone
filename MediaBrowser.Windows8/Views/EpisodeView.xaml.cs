using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;
using MediaBrowser.Windows8.Common;
using MediaBrowser.Windows8.ViewModel;
using Windows.UI.Xaml.Controls;
using MediaBrowser.Model.Dto;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class EpisodeView : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public EpisodeView()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => Messenger.Default.Send(new NotificationMessage(Constants.TvEpisodePageLoadedMsg));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                DataContext = History.Current.GetLastItem<TvViewModel>(GetType());
            }
            else if (e.NavigationMode == NavigationMode.New)
            {
                var item = (BaseItemDto)e.Parameter;
                var vm = ViewModelLocator.GetTvViewModel(item.SeriesId);
                vm.Episode = item;
                DataContext = vm;
            }
        }
    }
}
