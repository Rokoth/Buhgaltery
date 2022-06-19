using Buhgaltery.Contract.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buhgaltery.Desktop.Services.Interfaces
{
    public interface IAuthService
    {
        bool IsAuth { get; }
    }

    public interface IDataService<T, F, U> where T : Entity where F : Filter<T>
    {
        Task<List<T>> Get(F filter);
        Task<T> GetItem(Guid id);
        Task<T> Add(T entity);
        Task<T> Update(U entity);
        Task<T> Delete(Guid id);
    }
}
