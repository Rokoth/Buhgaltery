using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomingController : Controller
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<Incoming, IncomingFilter> _getDataService;
        private readonly IAddDataService<Incoming, IncomingCreator> _addDataService;
        private readonly IUpdateDataService<Incoming, IncomingUpdater> _updateDataService;
        private readonly IDeleteDataService<Incoming> _deleteDataService;
        private readonly IGetDataService<IncomingHistory, IncomingHistoryFilter> _getHistoryDataService;

        public IncomingController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Incoming, IncomingFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Incoming, IncomingCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Incoming, IncomingUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Incoming>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<IncomingHistory, IncomingHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetListAsync([FromBody] IncomingFilter IncomingFilter)
        {           
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(IncomingFilter, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetHistoryAsync([FromBody] IncomingHistoryFilter filter)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _getHistoryDataService.GetAsync(filter, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetItem")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetItemAsync([FromBody] Guid id)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(id, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAsync([FromBody] IncomingCreator creator)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _addDataService.AddAsync(creator, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync([FromBody] IncomingUpdater updater)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _updateDataService.UpdateAsync(updater, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::UpdateAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync([FromBody] Guid id)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _deleteDataService.DeleteAsync(id, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса IncomingController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
