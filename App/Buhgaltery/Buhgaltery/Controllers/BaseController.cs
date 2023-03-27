using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Buhgaltery.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("[controller]")]
    public class BaseController<TModel, TFilter, TCreator, TUpdater, THistory, THistoryFilter>: Controller
        where TModel : Entity
        where TFilter : Filter<TModel>
        where THistory : EntityHistory
        where THistoryFilter : Filter<THistory>
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<BaseController<TModel, TFilter, TCreator, TUpdater, THistory, THistoryFilter>> _logger;
        protected readonly IGetDataService<TModel, TFilter> _getDataService;
        protected readonly IAddDataService<TModel, TCreator> _addDataService;
        protected readonly IUpdateDataService<TModel, TUpdater> _updateDataService;
        protected readonly IDeleteDataService<TModel> _deleteDataService;
        protected readonly IGetDataService<THistory, THistoryFilter> _getHistoryDataService;

        public BaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<BaseController<TModel, TFilter, TCreator, TUpdater, THistory, THistoryFilter>>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<TModel, TFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<TModel, TCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<TModel, TUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<TModel>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<THistory, THistoryFilter>>();
        }

        protected async Task<IActionResult> GetListInternalAsync(TFilter filter)
        {
            return await ExecuteAsync(
                (userid, cancToken) => _getDataService.GetAsync(filter, userid, cancToken),
                nameof(GetListInternalAsync));
        }

        protected async Task<IActionResult> GetHistoryInternalAsync([FromBody] THistoryFilter filter)
        {
            return await ExecuteAsync(
                (userid, cancToken) => _getHistoryDataService.GetAsync(filter, userid, cancToken),
                nameof(GetHistoryInternalAsync));           
        }

        protected async Task<IActionResult> GetItemInternalAsync([FromBody] Guid id)
        {
            return await ExecuteAsync(
                (userid, cancToken) => _getDataService.GetAsync(id, userid, cancToken),
                nameof(GetItemInternalAsync));            
        }

        protected async Task<IActionResult> AddInternalAsync([FromBody] TCreator creator)
        {
            return await ExecuteAsync(
                (userid, cancToken) => _addDataService.AddAsync(creator, userid, cancToken),
                nameof(AddInternalAsync));
        }

        protected async Task<IActionResult> UpdateInternalAsync([FromBody] TUpdater updater)
        {
            return await ExecuteAsync(
               (userid, cancToken) => _updateDataService.UpdateAsync(updater, userid, cancToken),
               nameof(UpdateInternalAsync));           
        }

        protected async Task<IActionResult> DeleteInternalAsync([FromBody] Guid id)
        {
            return await ExecuteAsync(
               (userid, cancToken) => _deleteDataService.DeleteAsync(id, userid, cancToken),
               nameof(UpdateInternalAsync));
        }

        protected async Task<IActionResult> ExecuteAsync<TResult>(Func<Guid, CancellationToken, Task<TResult>> execFunc, string method)
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await execFunc(userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса {nameof(TModel)}Controller::{method}: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}