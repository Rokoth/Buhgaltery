using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    public class AccountsController : Controller
    {
        public IAuthService _authService;
        public IServiceProvider _serviceProvider;

        public AccountsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = _serviceProvider.GetRequiredService<IAuthService>();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserIdentity userIdentity)
        {
           
        }
    }
}
