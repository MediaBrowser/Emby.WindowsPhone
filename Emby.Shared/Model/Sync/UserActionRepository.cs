using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Users;
using Emby.WindowsPhone.Extensions;
using ScottIsAFool.WindowsPhone.Extensions;

namespace Emby.WindowsPhone.Model.Sync
{
    public class UserActionRepository : IUserActionRepository
    {
        private const string ItemsFile = "Cache\\UserActions.json";
        private readonly IStorageServiceHandler _storageService;

        private List<UserAction> _items; 

        public UserActionRepository(IStorageService storageService)
        {
            _storageService = storageService.Local;
        }

        public Task Create(UserAction action)
        {
            return GetItemInternal(x => x.Id == action.Id);
        }

        public async Task Delete(UserAction action)
        {
            var item = await GetItemInternal(x => x.Id == action.Id);
            if (item != null)
            {
                _items.Remove(item);
                await SaveItems();
            }
        }

        public async Task<IEnumerable<UserAction>> Get(string serverId)
        {
            return await GetItemsInternal(x => x.ServerId == serverId);
        }

        private async Task LoadItems()
        {
            if (!_items.IsNullOrEmpty())
            {
                return;
            }

            var json = await _storageService.ReadStringIfFileExists(ItemsFile);
            var items = await json.DeserialiseAsync<List<UserAction>>();
            _items = items ?? new List<UserAction>();
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

        private async Task<List<UserAction>> GetItemsInternal(Func<UserAction, bool> func)
        {
            await LoadItems();

            var items = _items.Where(func);
            return items.ToList();
        }

        private async Task<UserAction> GetItemInternal(Func<UserAction, bool> func)
        {
            var items = await GetItemsInternal(func);
            return items.FirstOrDefault();
        }
    }
}