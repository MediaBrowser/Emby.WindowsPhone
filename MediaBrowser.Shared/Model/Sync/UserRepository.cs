using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.ApiInteraction.Data;
using MediaBrowser.Model.Dto;

namespace MediaBrowser.WindowsPhone.Model.Sync
{
    public class UserRepository : IUserRepository
    {
        public Task AddOrUpdate(string id, UserDto user)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserDto>> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}