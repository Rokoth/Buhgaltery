using AutoMapper;
using Buhgaltery.Contract.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ReservesRevisorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning = false;

        public ReservesRevisorHostedService(IServiceProvider serviceProvider)
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
            var _reserveUpdateDataService = serviceProvider.GetRequiredService<IUpdateDataService<Reserve, ReserveUpdater>>();
            var _reserveDataService = serviceProvider.GetRequiredService<IGetDataService<Reserve, ReserveFilter>>();
            var _productDataService = serviceProvider.GetRequiredService<IGetDataService<Product, ProductFilter>>();
            var _mapper = serviceProvider.GetRequiredService<IMapper>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            while (_isRunning)
            {
                try
                {
                    var allUsers = await _userDataService.GetAsync(new UserFilter(null, null, null, null), cancellationTokenSource.Token);
                    foreach (var user in allUsers.Data)
                    {
                        var reserves = await _reserveDataService.GetAsync(new ReserveFilter(null, null, null, user.Id, null), cancellationTokenSource.Token);
                        var products = await _productDataService.GetAsync(new ProductFilter(null, null, null, user.Id, null, null, true, null), cancellationTokenSource.Token);
                        foreach (var reserve in reserves.Data)
                        {
                            var product = products.Data.Single(s=>s.Id == reserve.ProductId);
                            if (reserve.Value > product.MaxValue)
                            {
                                var reserveUpdater = _mapper.Map<ReserveUpdater>(reserve);
                                reserveUpdater.Value = product.MaxValue;
                                await _reserveUpdateDataService.UpdateAsync(reserveUpdater, cancellationTokenSource.Token);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при ревизии резервов: {ex.Message} {ex.StackTrace}");
                }
            }
            cancellationTokenSource.Cancel();
        }
    }
}
