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
    public class OutgoingController : Controller
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<Outgoing, OutgoingFilter> _getDataService;
        private readonly IAddDataService<Outgoing, OutgoingCreator> _addDataService;
        private readonly IUpdateDataService<Outgoing, OutgoingUpdater> _updateDataService;
        private readonly IDeleteDataService<Outgoing> _deleteDataService;
        private readonly IGetDataService<OutgoingHistory, OutgoingHistoryFilter> _getHistoryDataService;

        public OutgoingController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Outgoing, OutgoingFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Outgoing, OutgoingCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Outgoing, OutgoingUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Outgoing>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<OutgoingHistory, OutgoingHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        
        public async Task<IActionResult> GetListAsync([FromBody] OutgoingFilter OutgoingFilter)
        {           
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(OutgoingFilter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]
        
        public async Task<IActionResult> GetHistoryAsync([FromBody] OutgoingHistoryFilter filter)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getHistoryDataService.GetAsync(filter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetItem")]
        
        public async Task<IActionResult> GetItemAsync([FromBody] Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(id, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        
        public async Task<IActionResult> AddAsync([FromBody] OutgoingCreator creator)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _addDataService.AddAsync(creator, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        
        public async Task<IActionResult> UpdateAsync([FromBody] OutgoingUpdater updater)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _updateDataService.UpdateAsync(updater, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::UpdateAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Delete")]
        
        public async Task<IActionResult> DeleteAsync([FromBody] Guid id)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _deleteDataService.DeleteAsync(id, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса OutgoingController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
