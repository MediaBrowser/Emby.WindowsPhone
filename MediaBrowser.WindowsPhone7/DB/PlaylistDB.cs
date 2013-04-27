using System.Collections.Generic;
using MediaBrowser.Shared;
using Wintellect.Sterling.Database;

namespace MediaBrowser.WindowsPhone.DB
{
    public class PlaylistDB : BaseDatabaseInstance
    {
        public override string Name
        {
            get
            {
                return "MediaBrowserPlaylist";
            }
        }

        protected override List<ITableDefinition> RegisterTables()
        {
            return new List<ITableDefinition>
                       {
                           CreateTableDefinition<PlaylistItem, int>(x => x.Id)
                       };
        }
    }
}
