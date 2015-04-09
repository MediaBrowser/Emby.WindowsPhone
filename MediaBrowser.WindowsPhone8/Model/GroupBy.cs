using System.ComponentModel;

namespace Emby.WindowsPhone.Model
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
