using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Shared;
using MediaBrowser.WindowsPhone.Model;

namespace MediaBrowser.WindowsPhone
{
    public class SpecificSettings : INotifyPropertyChanged
    {
        
        public bool IncludeTrailersInRecent { get; set; }

        public DeviceSettings DeviceSettings { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
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
