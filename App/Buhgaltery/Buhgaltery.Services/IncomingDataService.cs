using System;
using System.Linq.Expressions;

namespace Buhgaltery.Services
{
    public class IncomingDataService : DataService<Db.Model.Incoming, Contract.Model.Incoming,
       Contract.Model.IncomingFilter, Contract.Model.IncomingCreator, Contract.Model.IncomingUpdater>
    {
        public IncomingDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.Incoming, bool>> GetFilter(Contract.Model.IncomingFilter filter)
        {
            return s => filter.UserId == s.UserId
                && (string.IsNullOrEmpty(filter.Description) || s.Description.Contains(filter.Description))
                && (filter.DateFrom == null || s.IncomingDate >= filter.DateFrom.Value)
                && (filter.DateTo == null || s.IncomingDate <= filter.DateTo.Value);
        }
              

        protected override Db.Model.Incoming UpdateFillFields(Contract.Model.IncomingUpdater entity, Db.Model.Incoming entry)
        {
            entry.Description = entity.Description;
            entry.IncomingDate = entity.IncomingDate;
            entry.Value = entity.Value;
            return entry;
        }

        protected override string DefaultSort => "IncomingDate";

    }
}
