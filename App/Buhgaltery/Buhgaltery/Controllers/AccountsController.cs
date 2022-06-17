//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Buhgaltery.Common;
using Buhgaltery.Contract.Model;
using Buhgaltery.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AccountsController> _logger;
        private readonly AuthOptions _authOptions;
        private const int _tokenDelay = 30 * 1000;
        private const string _invalidMessage = "Invalid username or password.";
        private const string _errorMessage = "Ошибка при обработке запроса:";

        public AccountsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = _serviceProvider.GetRequiredService<IAuthService>();
            _logger = _serviceProvider.GetRequiredService<ILogger<AccountsController>>();
            _authOptions = _serviceProvider.GetRequiredService<IOptions<CommonOptions>>().Value.AuthOptions;
        }

        /// <summary>
        /// Login method
        /// </summary>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        [HttpPost("Login")]       
        public async Task<IActionResult> Login([FromBody] UserIdentity userIdentity)
        {           
            try
            {
                var source = new CancellationTokenSource(_tokenDelay);

                var identity = await _authService.AuthApi(userIdentity, source.Token);
                if (identity == null)
                {
                    return BadRequest(new { errorText = _invalidMessage });
                }                              

                return Ok(new ClientIdentityResponse
                {
                    Token = CreateJWT(identity),
                    UserName = identity.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_errorMessage} {ex.Message} StackTrace: {ex.StackTrace}");
                return BadRequest($"{_errorMessage} {ex.Message}");
            }
        }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private string CreateJWT(System.Security.Claims.ClaimsIdentity identity)
        {
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(_authOptions.LifeTime)),
                    signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }
    }
}
