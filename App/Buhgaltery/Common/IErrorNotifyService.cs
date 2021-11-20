using System.Threading.Tasks;

namespace Buhgaltery.Common
{
    public interface IErrorNotifyService
    {        
        Task Send(string message, MessageLevelEnum level = MessageLevelEnum.Error, string title = null);
    }
}