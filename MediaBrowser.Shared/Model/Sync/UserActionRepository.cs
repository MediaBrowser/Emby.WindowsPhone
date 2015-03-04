using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Users;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class UserActionRepository : IUserActionRepository
    {
        public Task Create(UserAction action)
        {
            throw new NotImplementedException();
        }

        public Task Delete(UserAction action)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserAction>> Get(string serverId)
        {
            throw new NotImplementedException();
        }
    }
}