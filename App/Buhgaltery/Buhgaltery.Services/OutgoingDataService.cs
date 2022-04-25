using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class OutgoingDataService : DataService<Db.Model.Outgoing, Contract.Model.Outgoing,
       Contract.Model.OutgoingFilter, Contract.Model.OutgoingCreator, Contract.Model.OutgoingUpdater>
    {
        public OutgoingDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.Outgoing, bool>> GetFilter(Contract.Model.OutgoingFilter filter)
        {
            return s => filter.UserId == s.UserId
               && (string.IsNullOrEmpty(filter.Description) || s.Description.Contains(filter.Description))
               && (filter.DateFrom == null || s.OutgoingDate >= filter.DateFrom.Value)
               && (filter.DateTo == null || s.OutgoingDate <= filter.DateTo.Value)
                && (filter.ProductId == null || s.ProductId == filter.ProductId.Value);
        }

        protected override async Task PrepareBeforeAdd(Db.Interface.IRepository<Db.Model.Outgoing> repository,
            Contract.Model.OutgoingCreator creator, CancellationToken token)
        {
            var reserveDataService = _serviceProvider.GetRequiredService<IAddDataService<Contract.Model.Reserve, Contract.Model.ReserveCreator>>();
            await reserveDataService.AddAsync(new Contract.Model.ReserveCreator() { 
               ProductId = creator.ProductId,
               UserId = creator.UserId,
               Value = -creator.Value
            }, token);
        }

        protected override async Task PrepareBeforeUpdate(Db.Interface.IRepository<Db.Model.Outgoing> repository,
            Contract.Model.OutgoingUpdater entity, CancellationToken token)
        {
            var reserveDataService = _serviceProvider.GetRequiredService<IAddDataService<Contract.Model.Reserve, Contract.Model.ReserveCreator>>();
            var currentEntity = await repository.GetAsync(entity.Id, token);
            if (currentEntity.ProductId != entity.ProductId)
            {
                await reserveDataService.AddAsync(new Contract.Model.ReserveCreator()
                {
                    ProductId = currentEntity.ProductId,
                    UserId = currentEntity.UserId,
                    Value = currentEntity.Value
                }, token);
                await reserveDataService.AddAsync(new Contract.Model.ReserveCreator()
                {
                    ProductId = entity.ProductId,
                    UserId = entity.UserId,
                    Value = -entity.Value
                }, token);
                return;
            }

            if (currentEntity.Value != entity.Value)
            {
                await reserveDataService.AddAsync(new Contract.Model.ReserveCreator()
                {
                    ProductId = currentEntity.ProductId,
                    UserId = currentEntity.UserId,
                    Value = currentEntity.Value - entity.Value
                }, token);               
            }
        }

        protected override async Task PrepareBeforeDelete(Db.Interface.IRepository<Db.Model.Outgoing> repository,
            Db.Model.Outgoing entity, CancellationToken token)
        {
            var reserveDataService = _serviceProvider.GetRequiredService<IAddDataService<Contract.Model.Reserve, Contract.Model.ReserveCreator>>();
            var currentEntity = await repository.GetAsync(entity.Id, token);
            await reserveDataService.AddAsync(new Contract.Model.ReserveCreator()
            {
                ProductId = currentEntity.ProductId,
                UserId = currentEntity.UserId,
                Value = currentEntity.Value
            }, token);
        }

        protected override Db.Model.Outgoing UpdateFillFields(Contract.Model.OutgoingUpdater entity, Db.Model.Outgoing entry)
        {
            entry.Description = entity.Description;
            entry.OutgoingDate = entity.OutgoingDate;
            entry.ProductId = entity.ProductId;
            entry.Value = entity.Value;
            return entry;
        }

        protected override string DefaultSort => "OutgoingDate";

    }
}
