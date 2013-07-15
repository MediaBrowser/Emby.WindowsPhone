using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Controls;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace MediaBrowser.Windows8.Views
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class SelectProfileView : MediaBrowser.Windows8.Common.LayoutAwarePage
    {
        public SelectProfileView()
        {
            this.InitializeComponent();
            Loaded +=
                (s, e) =>
                Messenger.Default.Send(new NotificationMessage(Constants.Messages.ProfileViewLoadedMsg));
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
        }

    }
}
