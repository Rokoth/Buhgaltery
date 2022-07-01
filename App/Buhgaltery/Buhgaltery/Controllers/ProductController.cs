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
    public class ProductController : Controller
    {        
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly IGetDataService<Product, ProductFilter> _getDataService;
        private readonly IAddDataService<Product, ProductCreator> _addDataService;
        private readonly IUpdateDataService<Product, ProductUpdater> _updateDataService;
        private readonly IDeleteDataService<Product> _deleteDataService;
        private readonly IGetDataService<ProductHistory, ProductHistoryFilter> _getHistoryDataService;

        public ProductController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;           
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _getDataService = _serviceProvider.GetRequiredService<IGetDataService<Product, ProductFilter>>();
            _addDataService = _serviceProvider.GetRequiredService<IAddDataService<Product, ProductCreator>>();
            _updateDataService = _serviceProvider.GetRequiredService<IUpdateDataService<Product, ProductUpdater>>();
            _deleteDataService = _serviceProvider.GetRequiredService<IDeleteDataService<Product>>();
            _getHistoryDataService = _serviceProvider.GetRequiredService<IGetDataService<ProductHistory, ProductHistoryFilter>>();
        }

        [Authorize]
        [HttpPost("GetList")]
        
        public async Task<IActionResult> GetListAsync([FromBody] ProductFilter ProductFilter)
        {           
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(ProductFilter, userId, source.Token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса ProductController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("GetListHuman")]

        public async Task<IActionResult> GetListHumanAsync()
        {
            try
            {
                var userId = Guid.Parse(User.Identity.Name);
                var source = new CancellationTokenSource(30000);
                var response = await _getDataService.GetAsync(new ProductFilter(null, null, null, null, null, false, null, null), userId, source.Token);
                var tree = GetChilds(response.Data.ToList(), null);

                return Ok(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке запроса ProductController::GetListAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        private List<ProductHuman> GetChilds(List<Product> allProducts, Guid? id)
        {
            return allProducts.Where(s=>s.ParentId == id).Select(s=>new ProductHuman() { 
               AddPeriod = s.AddPeriod,
               Childs = GetChilds(allProducts, s.Id),
               Description = s.Description,
               FullName = s.FullName,
               Id = s.Id,
               LastAddDate = s.LastAddDate,
               MaxValue = s.MaxValue,
               MinValue = s.MinValue,
               Name = s.Name,
               Reserve = s.Reserve
            }).ToList();
        }

        [Authorize]
        [HttpPost("GetHistory")]
        
        public async Task<IActionResult> GetHistoryAsync([FromBody] ProductHistoryFilter filter)
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
                _logger.LogError($"Ошибка при обработке запроса ProductController::GetHistoryAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса ProductController::GetItemAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Add")]
        
        public async Task<IActionResult> AddAsync([FromBody] ProductCreator creator)
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
                _logger.LogError($"Ошибка при обработке запроса ProductController::AddAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Update")]
        
        public async Task<IActionResult> UpdateAsync([FromBody] ProductUpdater updater)
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
                _logger.LogError($"Ошибка при обработке запроса ProductController::UpdateAsync: {ex.Message} {ex.StackTrace}");
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
                _logger.LogError($"Ошибка при обработке запроса ProductController::DeleteAsync: {ex.Message} {ex.StackTrace}");
                return BadRequest($"Ошибка при обработке запроса: {ex.Message}");
            }
        }
    }
}
