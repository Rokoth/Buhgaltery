using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public interface IAllocateReservesService
    {
        Task Execute(CancellationToken token);
    }
}