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
    public class FormulaController : Controller
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<Formula, FormulaFilter> _getDataService;
        private readonly IAddDataService<Formula, FormulaCreator> _addDataService;
        private readonly IUpdateDataService<Formula, FormulaUpdater> _updateDataService;
        private readonly IDeleteDataService<Formula> _deleteDataService;
        private readonly IGetDataService<FormulaHistory, FormulaHistoryFilter> _getHistoryDataService;

        public FormulaController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Formula, FormulaFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Formula, FormulaCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Formula, FormulaUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Formula>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<FormulaHistory, FormulaHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetListAsync([FromBody] FormulaFilter FormulaFilter)
        {           
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(FormulaFilter, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса FormulaController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetHistoryAsync([FromBody] FormulaHistoryFilter filter)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _getHistoryDataService.GetAsync(filter, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса FormulaController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса FormulaController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAsync([FromBody] FormulaCreator creator)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _addDataService.AddAsync(creator, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса FormulaController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync([FromBody] FormulaUpdater updater)
        {
            try
            {
                var source = new CancellationTokenSource(30000);
                var response = await _updateDataService.UpdateAsync(updater, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса FormulaController::UpdateAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса FormulaController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
