using System;
using System.Collections.Generic;
using MediaBrowser.Model.Sync;
using Emby.WindowsPhone.Services;

namespace Emby.WindowsPhone.Helpers
{
    public class SyncRequestHelper
    {
        public static SyncJobRequest CreateRequest(List<string> itemIds, string name = null)
        {
            var request = new SyncJobRequest
            {
                ItemIds = itemIds,
                UserId = AuthenticationService.Current.LoggedInUserId,
                Name = !string.IsNullOrEmpty(name) ? name : Guid.NewGuid().ToString(),
            };

            return request;
        }

        public static SyncJobRequest CreateRequest(string itemId, string name = null)
        {
            return CreateRequest(new List<string> {itemId}, name);
        }
    }
}
