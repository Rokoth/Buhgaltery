using Buhgaltery.Contract.Model;
using Buhgaltery.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buhgaltery.Desktop.Services
{
    public class AuthService : IAuthService
    {
        public bool IsAuth { get; }
    }


    public class SettingsDataService : IDataService<User, UserFilter, UserUpdater> 
    {
        private IServiceProvider _serviceProvider;

        public SettingsDataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<User> Add(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<User> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> Get(UserFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(UserUpdater entity)
        {
            throw new NotImplementedException();
        }
    }
}
