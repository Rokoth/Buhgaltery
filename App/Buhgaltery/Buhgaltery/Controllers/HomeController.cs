//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    /// <summary>
    /// Контроллер с методами для главной страницы
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {      
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public HomeController(IServiceProvider serviceProvider, ILogger<HomeController> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
                
        /// <summary>
        /// Вывод общих сумм на главной странице
        /// </summary>
        /// <returns></returns>
        [HttpGet("current-data")]
        [Authorize]
        public async Task<IActionResult> GetCurrentData()
        {
            AllData result = new AllData();
            try
            {               
                var userId = Guid.Parse(User.Identity.Name);
                CancellationTokenSource source = new CancellationTokenSource(30000);
                var _incomingDataService = _serviceProvider.GetRequiredService<IGetDataService<Incoming, IncomingFilter>>();
                var _outgoingDataService = _serviceProvider.GetRequiredService<IGetDataService<Outgoing, OutgoingFilter>>();
                var _reserveDataService = _serviceProvider.GetRequiredService<IGetDataService<Reserve, ReserveFilter>>();
                var _correctionDataService = _serviceProvider.GetRequiredService<IGetDataService<Correction, CorrectionFilter>>();
                var incomings = await _incomingDataService.GetAsync(new IncomingFilter(null, null, null, null, null, null), userId, source.Token);
                var outgoings = await _outgoingDataService.GetAsync(new OutgoingFilter(null, null, null, null, null, null, null), userId, source.Token);
                var reserves = await _reserveDataService.GetAsync(new ReserveFilter(null, null, null, null), userId, source.Token);
                var corrections = await _correctionDataService.GetAsync(new CorrectionFilter(null, null, null, null, null, null), userId, source.Token);

                result.Incomings = incomings.Data.Sum(s=>s.Value);
                result.Outgoings = outgoings.Data.Sum(s => s.Value);
                result.Reserves = reserves.Data.Sum(s => s.Value);
                result.Corrections = corrections.Data.Sum(s => s.Value);
                result.Free = result.Incomings - result.Outgoings - result.Reserves + result.Corrections;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сумм");
                result.IsError = true;
                result.ErrorMessage = ex.Message;
            }
            return Ok(result);
        }        
    }
}
