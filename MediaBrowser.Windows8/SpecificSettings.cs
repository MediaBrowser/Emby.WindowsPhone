using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model.Dto;
using System.ComponentModel;
namespace MediaBrowser.Windows8
{
    public class SpecificSettings : INotifyPropertyChanged
    {
        public SpecificSettings()
        {
            ShowCastImages = ShowItemDetails = true;
            IncludeTrailersInRecentItems = true;
        }
        public bool ShowCastImages { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether item details should be shown in the FolderView.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show item details]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowItemDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include trailers in recent items].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include trailers in recent items]; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeTrailersInRecentItems { get; set; }

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
