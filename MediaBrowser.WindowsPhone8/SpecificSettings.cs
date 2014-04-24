using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;
#if WP8
using MediaBrowser.WindowsPhone.Services;
#endif

namespace MediaBrowser.WindowsPhone
{
    public class SpecificSettings : INotifyPropertyChanged
    {
        public SpecificSettings()
        {
            GroupByItems = Enum<GroupBy>.GetNames();
            DefaultGroupBy = GroupByItems.FirstOrDefault(x => x == DefaultGroupBy);
            EnableImageEnhancers = true;
#if WP8
            RecordedGroupByItems = Enum<RecordedGroupBy>.GetNames();
            DefaultRecordedGroupBy = RecordedGroupByItems.FirstOrDefault(x => x == DefaultRecordedGroupBy);
            UseRichWideTile = true;
            DisplayBackdropOnTile = true;
#endif
        }

        public bool IncludeTrailersInRecent { get; set; }
        public GroupBy DefaultGroupBy { get; set; }
        public List<GroupBy> GroupByItems { get; set; }
#if WP8
        public RecordedGroupBy DefaultRecordedGroupBy { get; set; }
        public List<RecordedGroupBy> RecordedGroupByItems { get; set; }
#endif
        public bool JustShowFolderView { get; set; }
        public bool ShowMissingEpisodes { get; set; }
        public bool ShowUnairedEpisodes { get; set; }
        public bool EnableImageEnhancers { get; set; }

        public DeviceSettings DeviceSettings { get; set; }

#if WP8
        public LockScreenType LockScreenType { get; set; }
        public string LockScreenCollectionId { get; set; }
        public bool DisplayBackdropOnTile { get; set; }
        public bool UseRichWideTile { get; set; }

        public bool UseTransparentTile { get; set; }
        
        [UsedImplicitly]
        private void OnLockScreenCollectionIdChanged()
        {
            LockScreenService.Current.CollectionId = LockScreenCollectionId;
        }

        [UsedImplicitly]
        private async void OnLockScreenTypeChanged()
        {
            await LockScreenService.Current.SetLockScreen(LockScreenType);
        }
#endif
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        [UsedImplicitly]
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
                Messenger.Default.Send(new PropertyChangedMessage<object>(null, null, propertyName));
            }
        }
    }
}
