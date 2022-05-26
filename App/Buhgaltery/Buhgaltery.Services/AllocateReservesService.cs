using Buhgaltery.Contract.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class AllocateReservesService : IAllocateReservesService
    {
        private ILogger<AllocateReservesHostedService> _logger;
        private IGetDataService<User, UserFilter> _userDataService;
        private IUpdateDataService<User, UserUpdater> _userUpdateService;
        private IAddDataService<Reserve, ReserveCreator> _reserveDataService;

        public AllocateReservesService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<AllocateReservesHostedService>>();
            _userDataService = serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
            _userUpdateService = serviceProvider.GetRequiredService<IUpdateDataService<User, UserUpdater>>();
            _reserveDataService = serviceProvider.GetRequiredService<IAddDataService<Reserve, ReserveCreator>>();
        }

        public async Task Execute(CancellationToken token)
        {
            try
            {
                var allUsers = await _userDataService.GetAsync(new UserFilter(null, null, null, null), token);
                foreach (var user in allUsers.Data)
                {
                    if (!user.LastAddedDate.HasValue || (user.LastAddedDate.Value.AddMinutes(user.AddPeriod) < DateTimeOffset.Now))
                    {
                        await _reserveDataService.AddAsync(new ReserveCreator()
                        {
                            UserId = user.Id
                        }, token);
                        user.LastAddedDate = DateTimeOffset.Now;
                        await _userUpdateService.UpdateAsync(new UserUpdater() { 
                             Description = user.Description,
                             FormulaId = user.FormulaId,
                             Id = user.Id,
                             LastAddedDate = DateTimeOffset.Now,
                             LeafOnly = user.LeafOnly,
                             Login = user.Login,
                             Name = user.Name,
                             PasswordChanged = false
                        }, token);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при резервировании : {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
