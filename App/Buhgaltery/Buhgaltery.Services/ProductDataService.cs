using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ProductDataService : DataService<Db.Model.Product, Contract.Model.Product,
       Contract.Model.ProductFilter, Contract.Model.ProductCreator, Contract.Model.ProductUpdater>
    {
        public ProductDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.Product, bool>> GetFilter(Contract.Model.ProductFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name)) 
                     && (filter.LastAddDateFrom == null || s.LastAddDate >= filter.LastAddDateFrom)
                      && (filter.LastAddDateTo == null || s.LastAddDate < filter.LastAddDateTo)
                      && (!filter.LeafOnly || s.IsLeaf )
                      && (filter.ParentId == null || s.ParentId == filter.ParentId)
                      && (filter.UserId == s.UserId);
        }
                
        protected override Db.Model.Product UpdateFillFields(Contract.Model.ProductUpdater entity, Db.Model.Product entry)
        {           
            entry.AddPeriod = entity.AddPeriod;
            entry.Name = entity.Name;
            entry.Description = entity.Description;            
            entry.MaxValue = entity.MaxValue;
            entry.MinValue = entity.MinValue;
            entry.ParentId = entity.ParentId;        
            return entry;
        }

        protected override string DefaultSort => "Name";       

        protected override async Task PrepareBeforeUpdate(Db.Interface.IRepository<Db.Model.Product> repository, Contract.Model.ProductUpdater entity, CancellationToken token)
        {
            var currentEntity = await repository.GetAsync(entity.Id, token);
            if (currentEntity.ParentId!= entity.ParentId && currentEntity.ParentId.HasValue)
            {
                var otherChilds = await repository.GetAsync(new Db.Model.Filter<Db.Model.Product>() {
                  Selector = s=>s.ParentId == currentEntity.ParentId && s.Id != currentEntity.Id
                }, token);

                if (!otherChilds.Data.Any())
                {
                    var currentParent = await repository.GetAsync(currentEntity.ParentId.Value, token);
                    currentParent.IsLeaf = true;
                    currentParent.VersionDate = DateTime.Now;
                    await repository.UpdateAsync(currentParent, false, token);
                }
            }
        }

        protected override async Task PrepareBeforeDelete(Db.Interface.IRepository<Db.Model.Product> repository, Db.Model.Product entity, CancellationToken token)
        {
            var currentEntity = await repository.GetAsync(entity.Id, token);
            if (currentEntity.ParentId.HasValue)
            {
                var otherChilds = await repository.GetAsync(new Db.Model.Filter<Db.Model.Product>()
                {
                    Selector = s => s.ParentId == currentEntity.ParentId && s.Id != currentEntity.Id
                }, token);

                if (!otherChilds.Data.Any())
                {
                    var currentParent = await repository.GetAsync(currentEntity.ParentId.Value, token);
                    currentParent.IsLeaf = true;
                    currentParent.VersionDate = DateTime.Now;
                    await repository.UpdateAsync(currentParent, false, token);
                }
            }
        }

        protected override async Task ActionAfterAdd(Db.Interface.IRepository<Db.Model.Product> repository, Contract.Model.ProductCreator creator, Db.Model.Product entity, CancellationToken token)
        {
            if (entity.ParentId.HasValue)
            {
                var parent = await repository.GetAsync(entity.ParentId.Value, token);
                if (parent.IsLeaf)
                {
                    parent.IsLeaf = false;
                    parent.VersionDate = DateTime.Now;
                    await repository.UpdateAsync(parent, false, token);
                }
            }
        }

        protected override async Task ActionAfterUpdate(Db.Interface.IRepository<Db.Model.Product> repository, Contract.Model.ProductUpdater updater, Db.Model.Product entity, CancellationToken token)
        {
            if (entity.ParentId.HasValue)
            {
                var parent = await repository.GetAsync(entity.ParentId.Value, token);
                if (parent.IsLeaf)
                {
                    parent.IsLeaf = false;
                    parent.VersionDate = DateTime.Now;
                    await repository.UpdateAsync(parent, false, token);
                }
            }
        }     
    }
}
