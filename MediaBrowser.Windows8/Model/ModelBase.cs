using System;
using System.ComponentModel;

namespace MediaBrowser.Windows8.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
