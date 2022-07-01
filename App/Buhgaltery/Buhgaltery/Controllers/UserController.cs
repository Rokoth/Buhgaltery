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
    public class UserController : Controller
    {
        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<User, UserFilter> _getDataService;
        private readonly IAddDataService<User, UserCreator> _addDataService;
        private readonly IUpdateDataService<User, UserUpdater> _updateDataService;
        private readonly IDeleteDataService<User> _deleteDataService;
        private readonly IGetDataService<UserHistory, UserHistoryFilter> _getHistoryDataService;

        public UserController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<User, UserCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<User, UserUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<User>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<UserHistory, UserHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        
        public async Task<IActionResult> GetListAsync([FromBody] UserFilter userFilter)
        {           
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(userFilter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса UserController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetHistory")]
        
        public async Task<IActionResult> GetHistoryAsync([FromBody] UserHistoryFilter filter)
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
                _logger.LogError($"Ошибка при обработке запроса UserController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса UserController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        
        public async Task<IActionResult> AddAsync([FromBody] UserCreator creator)
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
                _logger.LogError($"Ошибка при обработке запроса UserController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdater updater)
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
                _logger.LogError($"Ошибка при обработке запроса UserController::UpdateAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса UserController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
