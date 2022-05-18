using Microsoft.Extensions.DependencyInjection;
using System;
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
                var settings = await _userSettingsRepository.GetAsync(new Db.Model.Filter<Db.Model.UserSettings>()
                { 
                  Selector = s=>s.UserId == creator.UserId
                }, token);

                if (!creator.Value.HasValue)
                {
                    
                }
                else
                {
                    value = creator.Value.Value;
                }

                if (!creator.ProductId.HasValue)
                {

                }
                else
                {
                    productId = creator.ProductId.Value;
                }

                var entity = MapToEntityAdd(creator);
                
                
                var result = await repo.AddAsync(entity, false, token);
                await ActionAfterAdd(repo, creator, result, token);
                await repo.SaveChangesAsync();
                var prepare = _mapper.Map<Contract.Model.Reserve>(result);
                prepare = await Enrich(prepare, token);
                return prepare;
            });
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
