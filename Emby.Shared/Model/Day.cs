using System;
using PropertyChanged;

namespace MediaBrowser.Model
{
    [ImplementPropertyChanged]
    public class Day
    {
        public DayOfWeek DayOfWeek { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelected { get; set; }
    }
}
