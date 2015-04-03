using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Messaging;
using MediaBrowser.WindowsPhone.Model.Interfaces;
using Emby.WindowsPhone.Localisation;
using MediaBrowser.WindowsPhone.ViewModel.Items;

namespace MediaBrowser.WindowsPhone.ViewModel.Sync
{
    public class SyncJobDetailViewModel : ViewModelBase
    {
        public SyncJobDetailViewModel(IConnectionManager connectionManager, INavigationService navigationService)
            : base(navigationService, connectionManager)
        {
            SelectedItems = new List<SyncJobItemViewModel>();
        }

        public ObservableCollection<SyncJobItemViewModel> SyncJobItems { get; set; }
        public SyncJobViewModel SyncJob { get; set; }
        public bool SelectionModeIsOn { get; set; }

        public int SelectedIndex
        {
            get { return SelectionModeIsOn ? 1 : 0; }
        }

        public RelayCommand TurnSelectionOnCommand
        {
            get
            {
                return new RelayCommand(() => SelectionModeIsOn = true);
            }
        }

        public List<SyncJobItemViewModel> SelectedItems { get; set; }

        public RelayCommand<SelectionChangedEventArgs> SelectionChangedCommand
        {
            get
            {
                return new RelayCommand<SelectionChangedEventArgs>(args =>
                {
                    if (args.AddedItems != null)
                    {
                        foreach (var job in args.AddedItems.Cast<SyncJobItemViewModel>())
                        {
                            SelectedItems.Add(job);
                        }
                    }

                    if (args.RemovedItems != null)
                    {
                        foreach (var job in args.RemovedItems.Cast<SyncJobItemViewModel>())
                        {
                            SelectedItems.Remove(job);
                        }
                    }
                });
            }
        }

        public RelayCommand PageLoadedCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(false);
                });
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await LoadData(true);
                });
            }
        }

        public RelayCommand DeleteItemsCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!SelectedItems.Any())
                    {
                        return;
                    }

                    SetProgressBar(AppResources.SysTrayDeleting);
                    try
                    {
                        var list = SelectedItems.Select(DeleteItem).ToList();
                        await Task.WhenAll(list);

                        SelectedItems.Clear();

                        SetProgressBar();

                        await LoadData(true);

                        if (!SyncJobItems.Any())
                        {
                            Messenger.Default.Send(new NotificationMessage(Constants.Messages.RefreshSyncJobsMsg));
                            NavigationService.GoBack();
                        }
                    }
                    catch (HttpException ex)
                    {
                        Utils.HandleHttpException("DeleteItem()", ex, NavigationService, Log);
                    }
                });
            }
        }

        private Task DeleteItem(SyncJobItemViewModel item)
        {
            return ApiClient.CancelSyncJobItem(item.SyncJobItem.Id);
        }

        private async Task LoadData(bool isRefresh)
        {
            try
            {
                SetProgressBar(AppResources.SysTrayGettingItems);

                var query = new SyncJobItemQuery
                {
                    JobId = SyncJob.Id,
                    TargetId = ConnectionManager.Device.DeviceId,
                    AddMetadata = true
                };
                var items = await ApiClient.GetSyncJobItems(query);

                if (items != null && !items.Items.IsNullOrEmpty())
                {
                    SyncJobItems = items.Items.Select(x => new SyncJobItemViewModel(x, NavigationService, ConnectionManager, this)).ToObservableCollection();
                }
            }
            catch (HttpException ex)
            {
                Utils.HandleHttpException("LoadData()", ex, NavigationService, Log);
            }

            SetProgressBar();
        }

        public override void WireMessages()
        {
            Messenger.Default.Register<SyncJobMessage>(this, m =>
            {
                SyncJob = m.SyncJob;
                SyncJobItems = null;
            });
        }
    }
}
