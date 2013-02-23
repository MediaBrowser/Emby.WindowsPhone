using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Model.Dto;
using System;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MediaBrowser.Windows8.SettingsViews
{
    public sealed partial class StreamingSettings : UserControl
    {
        public StreamingSettings()
        {
            this.InitializeComponent();
            //var list = Enum.GetValues(typeof (StreamingQuality));
            //var qualityList = list.Cast<StreamingQuality>().ToList();
            //VideoStreamingQuality.ItemsSource = qualityList;
            //VideoStreamingQuality.SelectedItem = qualityList.FirstOrDefault(x => x == App.SpecificSettings.VideoStreamingQuality);
        }
    }
}
