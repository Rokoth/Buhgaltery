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
                      && (filter.LeafOnly || s.LastAddDate < filter.LastAddDateTo)
                      && (filter.LastAddDateTo == null || s.LastAddDate < filter.LastAddDateTo)
                      && (filter.LastAddDateTo == null || s.LastAddDate < filter.LastAddDateTo)
                      && (filter.LastAddDateTo == null || s.LastAddDate < filter.LastAddDateTo);
        }

        protected override async Task PrepareBeforeAdd(Db.Interface.IRepository<Db.Model.Product> repository, 
            Contract.Model.ProductCreator creator, CancellationToken token)
        {
            if (creator.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new Db.Model.Filter<Db.Model.Product>()
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

        protected override async Task PrepareBeforeUpdate(Db.Interface.IRepository<Db.Model.Product> repository, 
            Contract.Model.ProductUpdater entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefaults = await repository.GetAsync(new Db.Model.Filter<Db.Model.Product>()
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

        protected override async Task PrepareBeforeDelete(Db.Interface.IRepository<Db.Model.Product> repository,
            Db.Model.Product entity, CancellationToken token)
        {
            if (entity.IsDefault)
            {
                var currentDefault = (await repository.GetAsync(new Db.Model.Filter<Db.Model.Product>()
                {
                    Page = 0,
                    Size = 10,
                    Selector = s => true
                }, token)).Data.FirstOrDefault();
                currentDefault.IsDefault = true;
                await repository.UpdateAsync(currentDefault, false, token);
            }
        }

        protected override Db.Model.Product UpdateFillFields(Contract.Model.ProductUpdater entity, Db.Model.Product entry)
        {
            entry.Text = entity.Text;
            entry.Name = entity.Name;
            entry.IsDefault = entity.IsDefault;
            return entry;
        }

        protected override string DefaultSort => "Name";

    }
}
