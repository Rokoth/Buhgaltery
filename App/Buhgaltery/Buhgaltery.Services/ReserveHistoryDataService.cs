using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class ReserveHistoryDataService : DataGetService<Db.Model.ReserveHistory, Contract.Model.ReserveHistory,
        Contract.Model.ReserveHistoryFilter>
    {
        public ReserveHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<Db.Model.Filter<Db.Model.ReserveHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<Db.Model.ReserveHistory>>> GetListFunc(Db.Interface.IRepository<Db.Model.ReserveHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<Db.Model.ReserveHistory, bool>> GetFilter(Contract.Model.ReserveHistoryFilter filter)
        {
            return s => (filter.Id == null || s.Id == filter.Id);
        }
    }
}
