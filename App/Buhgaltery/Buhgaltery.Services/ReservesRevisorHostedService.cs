using AutoMapper;
using Buhgaltery.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ReservesRevisorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;       
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _token;
        private bool _toRun = false;

        public ReservesRevisorHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            var options = serviceProvider.GetRequiredService<IOptions<CommonOptions>>();
            _toRun = options.Value.RunOptions.AllocateReserves;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {                       
            await Task.Factory.StartNew(() => Run(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }

        private async Task Run()
        {
            if (_toRun)
            {
                while (!_token.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceProvider = scope.ServiceProvider;
                        var service = serviceProvider.GetRequiredService<IReservesRevisorService>();
                        await service.CheckReserveValues(_token);
                        await service.CheckSum(_token);
                    }
                    await Task.Delay(5 * 60 * 1000, _token);
                }
            }
        }
    }
}
