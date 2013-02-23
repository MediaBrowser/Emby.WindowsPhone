using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using MediaBrowser.Model;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.System;

namespace MediaBrowser.Windows8.Model
{
    public class SettingsService : ISettingsService, INotifyPropertyChanged
    {
        public UserDto LoggedInUser { get; set; }
        public string PinCode { get; set; }
        public ConnectionDetails ConnectionDetails { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
        public SystemInfo SystemStatus { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
