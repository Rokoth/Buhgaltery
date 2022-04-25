using Buhgaltery.Contract.Model;
using Buhgaltery.Desktop.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buhgaltery.Desktop.Services
{
    public class AuthService : IAuthService
    {
        public bool IsAuth { get; }
    }


    //public class GetDataService<T> : IGetDataService<T> where T : Entity
    //{
    //    private IServiceProvider _serviceProvider;

    //    public GetDataService(IServiceProvider serviceProvider)
    //    {
    //        _serviceProvider = serviceProvider;
    //    }

    //    public async Task<List<T>> GetListAsync(int size, int page, string sort, Guid userId)
    //    { 
        
    //    }
    //}
}
