using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.WindowsPhone.Model;
using MediaBrowser.WindowsPhone.Model.Streaming;
using MediaBrowser.WindowsPhone.Services;

namespace MediaBrowser.WindowsPhone
{
    public class SpecificSettings : INotifyPropertyChanged
    {
        public SpecificSettings()
        {
            GroupByItems = Enum<GroupBy>.GetNames();
            DefaultGroupBy = GroupByItems.FirstOrDefault(x => x == DefaultGroupBy);
            EnableImageEnhancers = true;
            RecordedGroupByItems = Enum<RecordedGroupBy>.GetNames();
            DefaultRecordedGroupBy = RecordedGroupByItems.FirstOrDefault(x => x == DefaultRecordedGroupBy);
            UseRichWideTile = true;
            DisplayBackdropOnTile = true;
            StreamingQuality = StreamingQuality.FourEightyLow;
            WifiStreamingQuality = StreamingQuality.SevenTwentyMedium;
        }

        public bool IncludeTrailersInRecent { get; set; }
        public GroupBy DefaultGroupBy { get; set; }
        public List<GroupBy> GroupByItems { get; set; }
        public RecordedGroupBy DefaultRecordedGroupBy { get; set; }
        public List<RecordedGroupBy> RecordedGroupByItems { get; set; }
        public bool JustShowFolderView { get; set; }
        public bool ShowMissingEpisodes { get; set; }
        public bool ShowUnairedEpisodes { get; set; }
        public bool EnableImageEnhancers { get; set; }

        public LockScreenType LockScreenType { get; set; }
        public string LockScreenCollectionId { get; set; }
        public bool DisplayBackdropOnTile { get; set; }
        public bool UseRichWideTile { get; set; }

        public bool UseTransparentTile { get; set; }
        public StreamingQuality StreamingQuality { get; set; }
        public StreamingQuality WifiStreamingQuality { get; set; }
        public bool OnlyStreamOnWifi { get; set; }

        
        
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
