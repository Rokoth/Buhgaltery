using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class FormulaDataService : DataService<Db.Model.Formula, Contract.Model.Formula,
       Contract.Model.FormulaFilter, Contract.Model.FormulaCreator, Contract.Model.FormulaUpdater>
    {
        public FormulaDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.Formula, bool>> GetFilter(Contract.Model.FormulaFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                     && (filter.IsDefault == null || s.IsDefault == filter.IsDefault);
        }

        protected override async Task PrepareBeforeAdd(Db.Interface.IRepository<Db.Model.Formula> repository, 
            Contract.Model.FormulaCreator creator, CancellationToken token)
        {
            if (creator.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new Db.Model.Filter<Db.Model.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.IsDefault
                }, token);
                foreach (var item in currentDefaults.Data)
                {
                    item.IsDefault = false;
                    await repository.UpdateAsync(item, false,  token);
                }
            }
        }

        protected override async Task PrepareBeforeUpdate(Db.Interface.IRepository<Db.Model.Formula> repository, 
            Contract.Model.FormulaUpdater entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new Db.Model.Filter<Db.Model.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => s.IsDefault && s.Id != entity.Id
                }, token);
                foreach (var item in currentDefaults.Data)
                {
                    item.IsDefault = false;
                    await repository.UpdateAsync(item, false, token);
                }
            }
        }

        protected override async Task PrepareBeforeDelete(Db.Interface.IRepository<Db.Model.Formula> repository,
            Db.Model.Formula entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefault = (await repository.GetAsync(new Db.Model.Filter<Db.Model.Formula>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => true
                }, token)).Data.FirstOrDefault();
                currentDefault.IsDefault = true;
                await repository.UpdateAsync(currentDefault, false, token);
            }
        }

        protected override Db.Model.Formula UpdateFillFields(Contract.Model.FormulaUpdater entity, Db.Model.Formula entry)
        {
            entry.Text = entity.Text;
            entry.Name = entity.Name;
            entry.IsDefault = entity.IsDefault;
            return entry;
        }

        protected override string DefaultSort => "Name";

    }

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
                
                var entity = MapToEntityAdd(creator);
                
                
                var result = await repo.AddAsync(entity, false, token);
                await ActionAfterAdd(repo, creator, result, token);
                await repo.SaveChangesAsync();
                var prepare = _mapper.Map<Tdto>(result);
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
