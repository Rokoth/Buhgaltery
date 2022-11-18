using Buhgaltery.Desktop.Services.Interfaces;
using System;
using System.Text;

namespace Buhgaltery.Desktop.Services
{
    public class AuthService : IAuthService
    {
        private readonly IServiceProvider _serviceProvider;
        public AuthService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
                
        public bool IsAuth { get; private set; }

        public bool TryAuth()
        {
            try
            {
                IsAuth = true;
                return true;

            }
            catch (Exception)
            {

                IsAuth = false;
                return false;
            }

        }
    }
}
