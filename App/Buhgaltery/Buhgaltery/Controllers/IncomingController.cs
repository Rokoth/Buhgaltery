//Licensed under the Apache License, Version 2.0
//
//ref1
using Buhgaltery.Contract.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class IncomingController : BaseController<Incoming, IncomingFilter, IncomingCreator, IncomingUpdater, IncomingHistory, IncomingHistoryFilter>
    {
        public IncomingController(IServiceProvider serviceProvider): base(serviceProvider) { }
                
        [HttpPost("GetList")]        
        public async Task<IActionResult> GetListAsync([FromBody] IncomingFilter incomingFilter)
        {
            return await GetListInternalAsync(incomingFilter);
        }

        [HttpPost("GetHistory")]        
        public async Task<IActionResult> GetHistoryAsync([FromBody] IncomingHistoryFilter filter)
        {
            return await GetHistoryInternalAsync(filter);
        }

        [HttpPost("GetItem")]        
        public async Task<IActionResult> GetItemAsync([FromBody] Guid id)
        {
            return await GetItemInternalAsync(id);
        }

        [HttpPost("Add")]        
        public async Task<IActionResult> AddAsync([FromBody] IncomingCreator creator)
        {
            return await AddInternalAsync(creator);
        }

        [HttpPost("Update")]        
        public async Task<IActionResult> UpdateAsync([FromBody] IncomingUpdater updater)
        {
            return await UpdateInternalAsync(updater);
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteAsync([FromBody] Guid id)
        {
            return await DeleteInternalAsync(id);
        }
    }
}
