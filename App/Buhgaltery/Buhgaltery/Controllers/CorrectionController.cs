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
    [Route("[controller]")]
    public class CorrectionController : Controller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CorrectionController> _logger;
        private readonly IGetDataService<Correction, CorrectionFilter> _getDataService;
        private readonly IAddDataService<Correction, CorrectionCreator> _addDataService;
        private readonly IUpdateDataService<Correction, CorrectionUpdater> _updateDataService;
        private readonly IDeleteDataService<Correction> _deleteDataService;
        private readonly IGetDataService<CorrectionHistory, CorrectionHistoryFilter> _getHistoryDataService;

        public CorrectionController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<CorrectionController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Correction, CorrectionFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Correction, CorrectionCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Correction, CorrectionUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Correction>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<CorrectionHistory, CorrectionHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]

        public async Task<IActionResult> GetListAsync([FromBody] CorrectionFilter CorrectionFilter)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(CorrectionFilter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]

        public async Task<IActionResult> GetHistoryAsync([FromBody] CorrectionHistoryFilter filter)
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
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]

        public async Task<IActionResult> AddAsync([FromBody] CorrectionCreator creator)
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
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]

        public async Task<IActionResult> UpdateAsync([FromBody] CorrectionUpdater updater)
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
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::UpdateAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса CorrectionController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
