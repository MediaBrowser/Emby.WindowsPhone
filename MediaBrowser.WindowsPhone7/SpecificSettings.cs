using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using JetBrains.Annotations;
using MediaBrowser.Model;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone
{
    public class SpecificSettings : INotifyPropertyChanged
    {
        public SpecificSettings()
        {
            GroupByItems = Enum<GroupBy>.GetNames();
            DefaultGroupBy = GroupByItems.FirstOrDefault(x => x == DefaultGroupBy);
        }

        public bool IncludeTrailersInRecent { get; set; }
        public GroupBy DefaultGroupBy { get; set; }
        public List<GroupBy> GroupByItems { get; set; }
        public bool JustShowFolderView { get; set; }

        public DeviceSettings DeviceSettings { get; set; }
        
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
