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

    public interface IDataService<T>
    {
        Task<T> Get(Guid id);
    }
}
