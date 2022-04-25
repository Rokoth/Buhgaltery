using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Buhgaltery.Services
{
    public class IncomingHistoryDataService : DataGetService<Db.Model.IncomingHistory, Contract.Model.IncomingHistory,
        Contract.Model.IncomingHistoryFilter>
    {
        public IncomingHistoryDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override string DefaultSort => "Name";

        protected override Func<Db.Model.Filter<Db.Model.IncomingHistory>, CancellationToken,
            Task<Contract.Model.PagedResult<Db.Model.IncomingHistory>>> GetListFunc(Db.Interface.IRepository<Db.Model.IncomingHistory> repo)
        {
            return repo.GetAsyncDeleted;
        }

        protected override Expression<Func<Db.Model.IncomingHistory, bool>> GetFilter(Contract.Model.IncomingHistoryFilter filter)
        {
            return s => (filter.Id == null || s.Id == filter.Id);
        }
    }
}
