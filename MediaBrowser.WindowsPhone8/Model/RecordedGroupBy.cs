using System.ComponentModel;

namespace MediaBrowser.WindowsPhone.Model
{
    public enum RecordedGroupBy
    {
        [Description("Recorded Date")]
        RecordedDate,
        [Description("Show Name")]
        ShowName,
        Channel
    }
}