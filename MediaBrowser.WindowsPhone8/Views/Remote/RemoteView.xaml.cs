using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.WindowsPhone.Services;
using MediaBrowser.WindowsPhone.ViewModel.Remote;

namespace MediaBrowser.WindowsPhone.Views.Remote
{
    /// <summary>
    /// Description for RemoteView.
    /// </summary>
    public partial class RemoteView
    {
        /// <summary>
        /// Initializes a new instance of the RemoteView class.
        /// </summary>
        public RemoteView()
        {
            InitializeComponent();

            WireMessages();
        }

        private void WireMessages()
        {
            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.HideChapterWindowMsg))
                {
                    if (ChapterWindow.IsOpen)
                    {
                        ChapterWindow.IsOpen = false;
                    }
                }
            });
        }

        protected override void InitialiseOnBack()
        {
            ((RemoteViewModel) DataContext).IsPinned = TileService.Current.TileExists(Constants.Pages.Remote.RemoteView);
        }

        private void ChapterButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ChapterWindow.IsOpen)
            {
                ChapterWindow.IsOpen = true;
            }
        }
    }
}