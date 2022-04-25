using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ProductHistoryDataService : DataGetService<Db.Model.ProductHistory, Contract.Model.ProductHistory,
        Contract.Model.ProductHistoryFilter>
    {
        public ProductHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<Db.Model.Filter<Db.Model.ProductHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<Db.Model.ProductHistory>>> GetListFunc(Db.Interface.IRepository<Db.Model.ProductHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<Db.Model.ProductHistory, bool>> GetFilter(Contract.Model.ProductHistoryFilter filter)
        {
            return s => (filter.Name == null || s.Name.Contains(filter.Name))
                && (filter.Id == null || s.Id == filter.Id);
        }
    }
}
