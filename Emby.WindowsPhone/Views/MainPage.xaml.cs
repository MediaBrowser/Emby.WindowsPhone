using System.Windows;
using Emby.WindowsPhone.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace Emby.WindowsPhone.Views
{
    public partial class MainPage
    {
        private readonly PanoramaItem _inProgressPano;
        private readonly PanoramaItem _userViewsPano;
        private readonly PanoramaItem _libraryViewsPano;
        private readonly PanoramaItem _libraryFoldersPano;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            _inProgressPano = InProgressPano;
            _userViewsPano = UserViews;
            _libraryFoldersPano = LibraryFolders;
            _libraryViewsPano = LibraryViews;
            ShowHideInProgress(false);
            ShowHideViewPanos();

            Messenger.Default.Register<NotificationMessage>(this, m =>
            {
                if (m.Notification.Equals(Constants.Messages.ShowHideInProgressMsg))
                {
                    var showInProgress = (bool) m.Sender;
                    ShowHideInProgress(showInProgress);
                }

                if (m.Notification.Equals(Constants.Messages.UseLibraryFoldersMsg))
                {
                    ShowHideViewPanos();
                }
            });
        }

        private void ShowHideViewPanos()
        {
            var useLibraryFolders = App.SpecificSettings.UseLibraryFolders;

            if (useLibraryFolders)
            {
                RemovePano(_userViewsPano);
                AddPano(_libraryFoldersPano, 0);
                AddPano(_libraryViewsPano, 0);
            }
            else
            {
                RemovePano(_libraryFoldersPano);
                RemovePano(_libraryViewsPano);
                AddPano(_userViewsPano, 0);
            }
        }

        private void ShowHideInProgress(bool showInProgress)
        {
            if (showInProgress)
            {
                AddPano(_inProgressPano);
            }
            else
            {
                RemovePano(_inProgressPano);
            }
        }

        private void AddPano(PanoramaItem item, int? atPosition = null)
        {
            if (MainPano.Items.IndexOf(item) < 0)
            {
                if (atPosition.HasValue)
                {
                    MainPano.Items.Insert(atPosition.Value, item);
                }
                else
                {
                    MainPano.Items.Add(item);
                }
            }
        }

        private void RemovePano(PanoramaItem item)
        {
            if (MainPano.Items.IndexOf(item) > -1)
            {
                MainPano.Items.Remove(item);
            }
        }
    }
}
