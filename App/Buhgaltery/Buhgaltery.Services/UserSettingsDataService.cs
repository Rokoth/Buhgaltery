using System;
using System.Linq.Expressions;

namespace Buhgaltery.Services
{
    public class UserSettingsDataService : DataService<Db.Model.UserSettings, Contract.Model.UserSettings,
       Contract.Model.UserSettingsFilter, Contract.Model.UserSettingsCreator, Contract.Model.UserSettingsUpdater>
    {
        public UserSettingsDataService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        protected override Expression<Func<Db.Model.UserSettings, bool>> GetFilter(Contract.Model.UserSettingsFilter filter)
        {
            return s => s.UserId == filter.UserId;
        }        
        protected override Db.Model.UserSettings UpdateFillFields(Contract.Model.UserSettingsUpdater entity, Db.Model.UserSettings entry)
        {           
            entry.DefaultReserveValue = entity.DefaultReserveValue;
            entry.LeafOnly = entity.LeafOnly;                       
            return entry;
        }

        protected override string DefaultSort => "Name";

    }
}
