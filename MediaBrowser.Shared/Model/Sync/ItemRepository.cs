using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Sync;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class ItemRepository : IItemRepository
    {
        public Task AddOrUpdate(LocalItem item)
        {
            throw new NotImplementedException();
        }

        public Task<LocalItem> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetServerItemIds(string serverId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetItemTypes(string serverId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<LocalItem>> GetItems(LocalItemQuery query)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetAlbumArtists(string serverId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<LocalItemInfo>> GetTvSeries(string serverId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<LocalItemInfo>> GetPhotoAlbums(string serverId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}