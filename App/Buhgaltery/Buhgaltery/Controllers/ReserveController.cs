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
    public class ReserveController : Controller
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<Reserve, ReserveFilter> _getDataService;
        private readonly IAddDataService<Reserve, ReserveCreator> _addDataService;
        private readonly IUpdateDataService<Reserve, ReserveUpdater> _updateDataService;
        private readonly IDeleteDataService<Reserve> _deleteDataService;
        private readonly IGetDataService<ReserveHistory, ReserveHistoryFilter> _getHistoryDataService;

        public ReserveController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Reserve, ReserveFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Reserve, ReserveCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Reserve, ReserveUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Reserve>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<ReserveHistory, ReserveHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        
        public async Task<IActionResult> GetListAsync([FromBody] ReserveFilter ReserveFilter)
        {           
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(ReserveFilter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса ReserveController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]
        
        public async Task<IActionResult> GetHistoryAsync([FromBody] ReserveHistoryFilter filter)
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
                _logger.LogError($"Ошибка при обработке запроса ReserveController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса ReserveController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        
        public async Task<IActionResult> AddAsync([FromBody] ReserveCreator creator)
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
                _logger.LogError($"Ошибка при обработке запроса ReserveController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        
        public async Task<IActionResult> UpdateAsync([FromBody] ReserveUpdater updater)
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
                _logger.LogError($"Ошибка при обработке запроса ReserveController::UpdateAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса ReserveController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
