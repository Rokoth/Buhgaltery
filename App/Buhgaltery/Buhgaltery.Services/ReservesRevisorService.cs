using Buhgaltery.Contract.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ReservesRevisorService : IReservesRevisorService
    {
       
        private readonly IGetDataService<User, UserFilter> _userDataService;       
        private readonly IAddDataService<Reserve, ReserveCreator> _reserveAddDataService;
        private readonly IGetDataService<Reserve, ReserveFilter> _reserveDataService;
        private readonly IGetDataService<Incoming, IncomingFilter> _incomingDataService;
        private readonly IGetDataService<Outgoing, OutgoingFilter> _outgoingDataService;
        private readonly IGetDataService<Correction, CorrectionFilter> _correctionDataService;

        private readonly IGetDataService<Product, ProductFilter> _productDataService;       
        private readonly ILogger<AllocateReservesHostedService> _logger;

        public ReservesRevisorService(IServiceProvider serviceProvider)
        {           
            _userDataService = serviceProvider.GetRequiredService<IGetDataService<User, UserFilter>>();            
            _reserveAddDataService = serviceProvider.GetRequiredService<IAddDataService<Reserve, ReserveCreator>>();
            _reserveDataService = serviceProvider.GetRequiredService<IGetDataService<Reserve, ReserveFilter>>();
            _productDataService = serviceProvider.GetRequiredService<IGetDataService<Product, ProductFilter>>();
            _logger = serviceProvider.GetRequiredService<ILogger<AllocateReservesHostedService>>();
        }

        public async Task CheckReserveValues(CancellationToken token)
        {
            try
            {
                var allUsers = await _userDataService.GetAsync(new UserFilter(null, null, null, null), token);
                foreach (var user in allUsers.Data)
                {
                    var reserves = await _reserveDataService.GetAsync(new ReserveFilter(null, null, null, user.Id, null), token);
                    var products = await _productDataService.GetAsync(new ProductFilter(null, null, null, user.Id, null, null, true, null, null), token);
                    foreach (var reserve in reserves.Data)
                    {
                        var product = products.Data.Single(s => s.Id == reserve.ProductId);
                        if (reserve.Value > product.MaxValue)
                        {
                            await _reserveAddDataService.AddAsync(new ReserveCreator()
                            {
                                ProductId = product.Id,
                                UserId = user.Id,
                                Value = product.MaxValue - reserve.Value
                            }, token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при ревизии резервов в методе CheckReserveValues: {ex.Message} {ex.StackTrace}");
            }
        }

        public async Task CheckSum(CancellationToken token)
        {
            try
            {
                var allUsers = await _userDataService.GetAsync(new UserFilter(null, null, null, null), token);
                foreach (var user in allUsers.Data)
                {
                    var reserves = await _reserveDataService.GetAsync(new ReserveFilter(null, null, null, user.Id, null), token);                    
                    var incomings = await _incomingDataService.GetAsync(new IncomingFilter(null, null, null, user.Id, null, null, null), token);
                    var outgoings = await _outgoingDataService.GetAsync(new OutgoingFilter(null, null, null, user.Id, null, null, null, null), token);
                    var corrections = await _correctionDataService.GetAsync(new CorrectionFilter(null, null, null, user.Id, null, null, null), token);

                    var allSum = incomings.Data.Sum(s => s.Value)
                            + corrections.Data.Sum(s => s.Value)
                            - reserves.Data.Sum(s => s.Value)
                            - outgoings.Data.Sum(s => s.Value);

                    var reservesToCorrect = reserves.Data.Where(s => s.Value > 0).ToList();

                    while (allSum < 0)
                    {                        
                        var minReserve = reservesToCorrect.Min(s => s.Value);
                        var avgSum = allSum / reservesToCorrect.Count;

                        if (minReserve < avgSum)
                        {
                            allSum -= minReserve * reservesToCorrect.Count;
                            foreach (var reserve in reservesToCorrect)
                            {
                                reserve.Value -= minReserve;
                                await _reserveAddDataService.AddAsync(new ReserveCreator()
                                {
                                    ProductId = reserve.ProductId,
                                    UserId = user.Id,
                                    Value = reserve.Value
                                }, token);
                            }
                            reservesToCorrect = reservesToCorrect.Where(s => s.Value > 0).ToList();
                        }
                        else
                        {
                            allSum = 0;
                            foreach (var reserve in reservesToCorrect)
                            {
                                reserve.Value -= avgSum;
                                await _reserveAddDataService.AddAsync(new ReserveCreator() { 
                                  ProductId = reserve.ProductId,
                                  UserId = user.Id, 
                                  Value = reserve.Value
                                }, token);
                            }
                        }                        
                    }                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при ревизии резервов в методе CheckSum: {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
