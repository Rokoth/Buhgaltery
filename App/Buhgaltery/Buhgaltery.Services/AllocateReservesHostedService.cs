using AutoMapper;
using Buhgaltery.Contract.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class AllocateReservesHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning = false;

        public AllocateReservesHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            await Task.Factory.StartNew(() => Run(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_isRunning) _isRunning = false;
            await Task.CompletedTask;
        }

        private async Task Run()
        {
            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var _logger = serviceProvider.GetRequiredService<ILogger<AllocateReservesHostedService>>();
            var _userDataService = serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
            var _userUpdateService = serviceProvider.GetRequiredService<IUpdateDataService<User, UserUpdater>>();
            var _reserveDataService = serviceProvider.GetRequiredService<IAddDataService<Reserve, ReserveCreator>>();
            var _mapper = serviceProvider.GetRequiredService<IMapper>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            while (_isRunning)
            {
                try
                {
                    var allUsers = await _userDataService.GetAsync(new UserFilter(null, null, null, null), cancellationTokenSource.Token);
                    foreach (var user in allUsers.Data)
                    {
                        if (!user.LastAddedDate.HasValue || (user.LastAddedDate.Value.AddMinutes(user.AddPeriod) < DateTimeOffset.Now))
                        {
                            await _reserveDataService.AddAsync(new ReserveCreator()
                            {
                                UserId = user.Id
                            }, cancellationTokenSource.Token);
                            UserUpdater updater = _mapper.Map<UserUpdater>(user);
                            updater.LastAddedDate = DateTimeOffset.Now;
                            await _userUpdateService.UpdateAsync(updater, cancellationTokenSource.Token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при резервировании : {ex.Message} {ex.StackTrace}");
                }
            }
            cancellationTokenSource.Cancel();
        }
    }
}
