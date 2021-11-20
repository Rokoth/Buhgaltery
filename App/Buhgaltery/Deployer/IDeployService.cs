using System.Threading.Tasks;

namespace Buhgaltery.BuhgalteryDeployer
{
    public interface IDeployService
    {
        Task Deploy(int? num = null);
    }
}
