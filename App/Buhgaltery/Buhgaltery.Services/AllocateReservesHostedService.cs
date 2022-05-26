using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        private CancellationToken _token;
        CancellationTokenSource _cancellationTokenSource;

        public AllocateReservesHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
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
            using var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            IAllocateReservesService service = serviceProvider.GetRequiredService<IAllocateReservesService>();
            while (!_token.IsCancellationRequested)
            {
                await service.Execute(_token);
                await Task.Delay(60 * 1000, _token);
            }
        }
    }
}
