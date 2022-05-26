using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public interface IReservesRevisorService
    {
        Task CheckReserveValues(CancellationToken token);
        Task CheckSum(CancellationToken token);
    }
}