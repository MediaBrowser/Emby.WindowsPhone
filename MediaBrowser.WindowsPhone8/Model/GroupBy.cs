using System.ComponentModel;
using MediaBrowser.WindowsPhone.Resources;

namespace MediaBrowser.WindowsPhone.Model
{
    public enum GroupBy
    {
        [Description("Name")]
        Name,
        [Description("Production Year")]
        ProductionYear,
        [Description("Genre")]
        Genre,
        //[Description("Studio")]
        //Studio
    }
}
