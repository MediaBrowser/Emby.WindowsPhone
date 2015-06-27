using System.ComponentModel;

namespace Emby.WindowsPhone.Model
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