using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Phone.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Sync;
using MediaBrowser.WindowsPhone.Extensions;
using ScottIsAFool.WindowsPhone.Extensions;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class ItemRepository : IItemRepository
    {
        private const string ItemsFile = "Cache\\LocalItems.json";
        private readonly IAsyncStorageService _storageService;

        private List<LocalItem> _items;

        public ItemRepository(IAsyncStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task AddOrUpdate(LocalItem item)
        {
            var existingItem = await GetItemInternal(x => x.Id == item.Id);
            if (existingItem == null)
            {
                _items.Add(item);
            }
            else
            {
                item.CopyItem(existingItem);
            }

            await SaveItems();
        }

        public Task<LocalItem> Get(string id)
        {
            return GetItemInternal(item => item.Id == id);
        }

        public async Task Delete(string id)
        {
            var itemToDelete = await GetItemInternal(item => item.Id == id);
            if (itemToDelete != null)
            {
                _items.Remove(itemToDelete);
                await SaveItems();
            }
        }

        public async Task<List<string>> GetServerItemIds(string serverId)
        {
            var items = await GetItemsInternal(item => item.ServerId == serverId);
            return items.Select(x => x.ItemId).ToList();
        }

        public async Task<List<string>> GetItemTypes(string serverId, string userId)
        {
            var items = await GetItemsInternal(i => i.ServerId == serverId && i.UserIdsWithAccess.Contains(userId));
            return items.Select(x => x.Item.Type).ToList();
        }

        public async Task<List<LocalItem>> GetItems(LocalItemQuery query)
        {
            await LoadItems();
            var filteredItems = _items.Where(i => i.ServerId == query.ServerId);
            if (!string.IsNullOrWhiteSpace(query.AlbumArtist))
            {
                if (query.AlbumArtist == "Unknown")
                {
                    filteredItems = filteredItems.Where(i => i.Item.AlbumArtist == query.AlbumArtist || i.Item.AlbumArtist == null);
                }
                else
                    filteredItems = filteredItems.Where(i => i.Item.AlbumArtist == query.AlbumArtist);
            }
            if (!string.IsNullOrWhiteSpace(query.AlbumId))
                filteredItems = filteredItems.Where(i => i.Item.AlbumId == query.AlbumId);

            if (!string.IsNullOrWhiteSpace(query.SeriesId))
                filteredItems = filteredItems.Where(i => i.Item.SeriesId == query.SeriesId);

            if (!string.IsNullOrWhiteSpace(query.Type))
                filteredItems = filteredItems.Where(i => i.Item.Type == query.Type);

            if (!string.IsNullOrWhiteSpace(query.MediaType))
                filteredItems = filteredItems.Where(i => i.Item.MediaType == query.MediaType);

            filteredItems = filteredItems.Where(i => !query.ExcludeTypes.Contains(i.Item.Type));

            return filteredItems.ToList();
        }

        public async Task<List<string>> GetAlbumArtists(string serverId, string userId)
        {
            var items = await GetItemsInternal(i => i.ServerId == serverId && i.UserIdsWithAccess.Contains(userId) && i.Item.Type == "Audio");
            return items.Select(x => x.Item.AlbumArtist ?? "Unknown").Distinct().ToList();
        }

        public async Task<List<LocalItemInfo>> GetTvSeries(string serverId, string userId)
        {
            var items = await GetItemsInternal(i => i.ServerId == serverId &&
                                                    i.UserIdsWithAccess.Contains(userId) &&
                                                    i.Item.Type == "Episode" &&
                                                    i.Item.MediaType == "Video");
            var groupedItems = items.GroupBy(x => x.Item.SeriesId);
            return groupedItems.Select(g => new LocalItemInfo
            {
                Name = g.First().Item.SeriesName, 
                Id = g.First().Item.SeriesId, 
                ServerId = serverId, 
                PrimaryImageTag = g.First().Item.SeriesPrimaryImageTag
            }).ToList();
        }

        public async Task<List<LocalItemInfo>> GetPhotoAlbums(string serverId, string userId)
        {
            var items = await GetItemsInternal(i => i.ServerId == serverId &&
                                                    i.UserIdsWithAccess.Contains(userId) &&
                                                    i.Item.Type == "Photo");
            var groupedItems = items.GroupBy(x => x.Item.AlbumId);
            return groupedItems.Select(g => new LocalItemInfo
            {
                Name = g.First().Item.Album,
                Id = g.First().Item.AlbumId,
                ServerId = serverId,
                PrimaryImageTag = g.First().Item.AlbumPrimaryImageTag
            }).ToList();
        }

        private async Task LoadItems()
        {
            if (!_items.IsNullOrEmpty())
            {
                return;
            }

            var json = await _storageService.ReadStringIfFileExists(ItemsFile);
            var items = await json.DeserialiseAsync<List<LocalItem>>();
            _items = items;
        }

        private async Task SaveItems()
        {
            if (_items.IsNullOrEmpty())
            {
                return;
            }

            await _storageService.CreateDirectoryIfNotThere("Cache");

            var json = await _items.SerialiseAsync();
            await _storageService.WriteAllTextAsync(ItemsFile, json);
        }
        
        private async Task<List<LocalItem>> GetItemsInternal(Func<LocalItem, bool> func)
        {
            await LoadItems();

            var items = _items.Where(func);
            return items.ToList();
        }

        private async Task<LocalItem> GetItemInternal(Func<LocalItem, bool> func)
        {
            var items = await GetItemsInternal(func);
            return items.FirstOrDefault();
        }
    }
}