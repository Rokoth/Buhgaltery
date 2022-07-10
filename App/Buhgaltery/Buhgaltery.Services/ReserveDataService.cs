﻿using Buhgaltery.Db.Interface;
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

        protected override Expression<Func<Db.Model.Reserve, bool>> GetFilter(Contract.Model.ReserveFilter filter, Guid userId)
        {
            return s => (userId == s.UserId)
                     && (filter.ProductId == null || s.ProductId == filter.ProductId);
        }

        public override async Task<Contract.Model.Reserve> AddAsync(Contract.Model.ReserveCreator creator, Guid userId, CancellationToken token)
        {
            return await ExecuteAsync(async (repo) =>
            {
                Guid productId;
                decimal value;
                Db.Interface.IRepository<Db.Model.User> _userRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.User>>();
                var user = await _userRepository.GetAsync(userId, token);
                Db.Interface.IRepository<Db.Model.UserSettings> _userSettingsRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.UserSettings>>();
                var settings = (await _userSettingsRepository.GetAsync(new Db.Model.Filter<Db.Model.UserSettings>()
                { 
                  Selector = s=> s.UserId == userId
                }, token)).Data.FirstOrDefault();

                if (settings == null)
                {
                    throw new Exception($"Для пользователя {userId} не заданы настройки");
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
                        Selector = s => s.UserId == userId
                    }, token)).Data;

                    Db.Interface.IRepository<Db.Model.Reserve> _reserveRepository = _serviceProvider.GetRequiredService<Db.Interface.IRepository<Db.Model.Reserve>>();
                    var reserves = (await _reserveRepository.GetAsync(new Db.Model.Filter<Db.Model.Reserve>()
                    {
                        Selector = s => s.UserId == userId
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
                            if (product.LastAddDate.AddHours(product.AddPeriod) <= DateTimeOffset.Now)
                            {
                                var currReserve = reserves.FirstOrDefault(s => s.ProductId == product.Id);
                                if (currReserve != null)
                                {
                                    if (currReserve.Value < product.MaxValue)
                                    {
                                        forSelect.Add(Serialize(product));
                                    }
                                    else
                                    {
                                        product.AddPeriod++;
                                        product.VersionDate = DateTimeOffset.Now;
                                        product.LastAddDate = product.LastAddDate.AddHours(product.AddPeriod);
                                        await _productRepository.UpdateAsync(product, false, token);
                                    }
                                }
                                else if (product.MaxValue > 0)
                                {
                                    forSelect.Add(Serialize(product));
                                }
                            }
                        }

                        if (forSelect.Count == 0) return null;

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
                                        
                    if (reserve != null)
                    {
                        if (value > (selected.MaxValue - reserve.Value))
                        {
                            if (creator.ProductId.HasValue)
                            {
                                selected.MaxValue = reserve.Value + value;
                            }
                            else
                            {
                                value = selected.MaxValue - reserve.Value;
                            }
                        }
                        reserve.Value += value;
                        await UpdateProduct(creator.ProductId, settings.DefaultReserveValue, creator.Value, _productRepository, selected, reserve, token);

                        reserve.VersionDate = DateTimeOffset.Now;
                        await repo.UpdateAsync(reserve, false, token);
                    }
                    else
                    {
                        if (value > selected.MaxValue)
                        {
                            if (creator.ProductId.HasValue)
                            {
                                selected.MaxValue = value;
                            }
                            else
                            {
                                value = selected.MaxValue;
                            }
                        }
                        reserve = new Db.Model.Reserve()
                        {
                             Id = Guid.NewGuid(),
                             IsDeleted = false,
                             ProductId = productId,
                             UserId = userId,
                             Value = value,
                             VersionDate = DateTimeOffset.Now
                        };

                        await UpdateProduct(creator.ProductId, settings.DefaultReserveValue, creator.Value, _productRepository, selected, reserve, token);

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

        private static async Task UpdateProduct(Guid? creatorProductId, decimal defaultReserveValue, decimal? creatorValue, IRepository<Db.Model.Product> _productRepository, 
            Db.Model.Product selected, Db.Model.Reserve reserve, CancellationToken token)
        {
            if (creatorProductId.HasValue)
            {
                if (reserve.Value < 0)
                {
                    var count = (int)Math.Ceiling(-reserve.Value / defaultReserveValue);
                    selected.LastAddDate = selected.LastAddDate.AddHours(selected.AddPeriod * count);
                    selected.AddPeriod = Math.Max(selected.AddPeriod - count, 1);
                    reserve.Value += (defaultReserveValue * count);
                    selected.VersionDate = DateTimeOffset.Now;
                    await _productRepository.UpdateAsync(selected, false, token);
                }
                else if (!creatorValue.HasValue || creatorValue.Value > 0)
                {
                    var count = 1;
                    if (creatorValue.HasValue) count = (int)Math.Ceiling(creatorValue.Value / defaultReserveValue);
                    selected.LastAddDate = selected.LastAddDate.AddHours(selected.AddPeriod * count);
                    selected.AddPeriod = Math.Max(selected.AddPeriod - count, 1);
                    selected.VersionDate = DateTimeOffset.Now;
                    await _productRepository.UpdateAsync(selected, false, token);
                }
            }
            else
            {
                selected.LastAddDate = selected.LastAddDate.AddHours(selected.AddPeriod);               
                selected.VersionDate = DateTimeOffset.Now;
                await _productRepository.UpdateAsync(selected, false, token);
            }
        }

        protected override async Task<Contract.Model.Reserve> Enrich(Contract.Model.Reserve entity, CancellationToken token)
        {
            var productDataService = _serviceProvider.GetRequiredService<IGetDataService<Contract.Model.Product, Contract.Model.ProductFilter>>();
            var product = await productDataService.GetAsync(entity.ProductId, entity.UserId, token);
            entity.Product = product.FullName;
            return entity;
        }

        protected override async Task<IEnumerable<Contract.Model.Reserve>> Enrich(IEnumerable<Contract.Model.Reserve> entities, CancellationToken token)
        {
            if (entities.Any())
            {
                List<Contract.Model.Reserve> result = new List<Contract.Model.Reserve>();
                var productDataService = _serviceProvider.GetRequiredService<IGetDataService<Contract.Model.Product, Contract.Model.ProductFilter>>();
                var products = await productDataService.GetAsync(new Contract.Model.ProductFilter(null, null, null, null, null, false, null, null), entities.First().UserId, token);
                foreach (var item in entities)
                {
                    var product = products.Data.First(s => s.Id == item.ProductId);
                    item.Product = product.FullName;
                    result.Add(item);
                }
                return result;
            }
            return entities;
        }

        private static CalcRequestItem Serialize(Db.Model.Product product)
        {
            var prepare = JObject.FromObject(product);
            prepare.Add("AddHours", (DateTimeOffset.Now - product.LastAddDate).TotalHours);
            return new CalcRequestItem()
            {
                Id = product.Id,
                Fields = prepare.ToString()
            };
        }

        public override async Task<Contract.Model.Reserve> UpdateAsync(Contract.Model.ReserveUpdater creator, Guid userId, CancellationToken token)
        {
            throw new DataServiceException("Операция Update для резервов недопустима");
        }

        public override async Task<Contract.Model.Reserve> DeleteAsync(Guid id, Guid userId, CancellationToken token)
        {
            throw new DataServiceException("Операция Delete для резервов недопустима");
        }

        protected override Db.Model.Reserve UpdateFillFields(Contract.Model.ReserveUpdater entity, Db.Model.Reserve entry)
        {           
            return entry;
        }

        protected override Db.Model.Reserve AdditionalMapForAdd(Db.Model.Reserve entity, Contract.Model.ReserveCreator creator, Guid userId)
        {
            entity.UserId = userId;
            return entity;
        }

        protected override async Task<bool> CheckUser(Db.Model.Reserve entity, Guid userId)
        {
            await Task.CompletedTask;
            return entity.UserId == userId;
        }

        protected override string DefaultSort => "ProductId";

    }
}
