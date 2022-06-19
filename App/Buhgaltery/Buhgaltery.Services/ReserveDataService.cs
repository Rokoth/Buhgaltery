using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ReserveDataService : DataService<Db.Model.Reserve, Contract.Model.Reserve,
       Contract.Model.ReserveFilter, Contract.Model.ReserveCreator, Contract.Model.ReserveUpdater>
    {
       
        public ReserveDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.Reserve, bool>> GetFilter(Contract.Model.ReserveFilter filter)
        {
            return s => (filter.UserId == s.UserId)
                     && (filter.ProductId == null || s.ProductId == filter.ProductId);
        }

        public override async Task<Contract.Model.Reserve> AddAsync(Contract.Model.ReserveCreator creator, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                Guid productId;
                decimal value;
                Db.Interface.IRepository<Db.Model.User> _userRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.User>>();
                var user = await _userRepository.GetAsync(creator.UserId, token);
                Db.Interface.IRepository<Db.Model.UserSettings> _userSettingsRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.UserSettings>>();
                var settings = (await _userSettingsRepository.GetAsync(new Db.Model.Filter<Db.Model.UserSettings>()
                { 
                  Selector = s=> s.UserId == creator.UserId
                }, token)).Data.FirstOrDefault();

                if (settings == null)
                {
                    throw new Exception($"Для пользователя {creator.UserId} не заданы настройки");
                }

                if (!creator.Value.HasValue)
                {

                    value = settings.DefaultReserveValue;
                }
                else
                {
                    value = creator.Value.Value;
                }

                if(value != 0)
                {
                    Db.Interface.IRepository<Db.Model.Product> _productRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.Product>>();
                    var products = (await _productRepository.GetAsync(new Db.Model.Filter<Db.Model.Product>()
                    {
                        Selector = s => s.UserId == creator.UserId
                    }, token)).Data;

                    Db.Interface.IRepository<Db.Model.Reserve> _reserveRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.Reserve>>();
                    var reserves = (await _reserveRepository.GetAsync(new Db.Model.Filter<Db.Model.Reserve>()
                    {
                        Selector = s => s.UserId == creator.UserId
                    }, token)).Data;

                    if (!creator.ProductId.HasValue)
                    {               
                        if (settings.LeafOnly)
                        {
                            products = products.Where(s => s.IsLeaf);
                        }
                        List<CalcRequestItem> forSelect = new List<CalcRequestItem>();
                        foreach (var product in products)
                        {
                            var currReserve = reserves.FirstOrDefault(s => s.ProductId == product.Id);
                            if (currReserve != null)
                            {
                                if (currReserve.Value < product.MaxValue)
                                {
                                    forSelect.Add(Serialize(product));
                                }
                            }
                            else if (product.MaxValue > 0 && product.LastAddDate.AddHours(product.AddPeriod)<= DateTimeOffset.Now)
                            {
                                forSelect.Add(Serialize(product));
                            }
                        }

                        Db.Interface.IRepository<Db.Model.Formula> _formulaRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.Formula>>();
                        var formula = await _formulaRepository.GetAsync(user.FormulaId, token);

                        var calculator = _serviceProvider.GetRequiredService<ICalculator>();

                        var calcResult = (calculator.Calculate(new CalcRequest() { 
                          ChangeOnSelect = null,
                          Count = 1,
                          Formula = formula.Text,
                          Items = forSelect
                        })).FirstOrDefault();

                        if (calcResult == null)
                        {
                            return null;
                        }

                        productId = calcResult.Id;
                    }
                    else
                    {
                        productId = creator.ProductId.Value;
                    }

                    var selected = products.FirstOrDefault(s => s.Id == productId);
                    var reserve = reserves.FirstOrDefault(s => s.ProductId == productId);

                    selected.LastAddDate = DateTimeOffset.Now;
                    selected.VersionDate = DateTimeOffset.Now;
                    await _productRepository.UpdateAsync(selected, false, token);

                    if (reserve != null)
                    {
                        if (value > (selected.MaxValue - reserve.Value))
                        {
                            value = selected.MaxValue - reserve.Value;
                        }
                        reserve.Value += value;
                        reserve.VersionDate = DateTimeOffset.Now;
                        await repo.UpdateAsync(reserve, false, token);
                    }
                    else
                    {
                        if (value > selected.MaxValue)
                        {
                            value = selected.MaxValue;
                        }
                        reserve = new Db.Model.Reserve()
                        {
                             Id = Guid.NewGuid(),
                             IsDeleted = false,
                             ProductId = productId,
                             UserId = creator.UserId,
                             Value = value,
                             VersionDate = DateTimeOffset.Now
                        };
                        await repo.AddAsync(reserve, false, token);
                    }
                    await repo.SaveChangesAsync();
                                        
                    var prepare = _mapper.Map<Contract.Model.Reserve>(reserve);
                    prepare = await base.Enrich(prepare, token);
                    return prepare;
                }
                return null;
                
            });
        }

        private static CalcRequestItem Serialize(Db.Model.Product product)
        {
            var prepare = JObject.FromObject(product);
            prepare.Add("AddHours", (product.LastAddDate - DateTimeOffset.Now).TotalHours);
            return new CalcRequestItem()
            {
                Id = product.Id,
                Fields = prepare.ToString()
            };
        }

        public override async Task<Contract.Model.Reserve> UpdateAsync(Contract.Model.ReserveUpdater creator, CancellationToken token)
        {
            throw new DataServiceException("Операция Update для резервов недопустима");
        }

        public override async Task<Contract.Model.Reserve> DeleteAsync(Guid id, CancellationToken token)
        {
            throw new DataServiceException("Операция Delete для резервов недопустима");
        }

        protected override Db.Model.Reserve UpdateFillFields(Contract.Model.ReserveUpdater entity, Db.Model.Reserve entry)
        {           
            return entry;
        }

        protected override string DefaultSort => "ProductId";

    }
}
