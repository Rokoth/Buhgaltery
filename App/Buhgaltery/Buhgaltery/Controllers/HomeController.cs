using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    [ApiController]
    [Route("home")]
    public class HomeController : ControllerBase
    {      
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public HomeController(IServiceProvider serviceProvider, ILogger<HomeController> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
                
        [HttpGet("current-data")]
        public async Task<AllData> GetCurrentData()
        {
            AllData result = new AllData();
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var _incomingDataService = _serviceProvider.GetRequiredService<IGetDataService<Incoming, IncomingFilter>>();
                var _outgoingDataService = _serviceProvider.GetRequiredService<IGetDataService<Outgoing, OutgoingFilter>>();
                var _reserveDataService = _serviceProvider.GetRequiredService<IGetDataService<Reserve, ReserveFilter>>();
                var incomings = await _incomingDataService.GetAsync(new IncomingFilter(null, null, null, userId), source.Token);
                var outgoings = await _outgoingDataService.GetAsync(new OutgoingFilter(null, null, null, userId), source.Token);
                var reserves = await _reserveDataService.GetAsync(new ReserveFilter(null, null, null, userId, null), source.Token);

                result.Incomings = incomings.Data.Sum(s=>s.Value);
                result.Outgoings = outgoings.Data.Sum(s => s.Value);
                result.Reserves = reserves.Data.Sum(s => s.Value);
                result.Free = result.Incomings - result.Outgoings - result.Reserves;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сумм");
                result.IsError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        
    }
}
