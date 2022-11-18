using AutoMapper;

namespace Buhgaltery
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Db.Model.User, Contract.Model.User>();
            CreateMap<Contract.Model.UserCreator, Db.Model.User>()
                .ForMember(s => s.Password, s => s.Ignore());
            CreateMap<Db.Model.UserHistory, Contract.Model.UserHistory>();
            CreateMap<Contract.Model.UserUpdater, Db.Model.User>()
                .ForMember(s => s.Password, s => s.Ignore());

            CreateMap<Db.Model.Formula, Contract.Model.Formula>();
            CreateMap<Contract.Model.FormulaCreator, Db.Model.Formula>();
            CreateMap<Db.Model.FormulaHistory, Contract.Model.FormulaHistory>();
            CreateMap<Contract.Model.FormulaUpdater, Db.Model.Formula>();

            CreateMap<Db.Model.Product, Contract.Model.Product>();
            CreateMap<Contract.Model.ProductCreator, Db.Model.Product>();
            CreateMap<Db.Model.ProductHistory, Contract.Model.ProductHistory>();
            CreateMap<Contract.Model.ProductUpdater, Db.Model.Product>();

            CreateMap<Db.Model.Incoming, Contract.Model.Incoming>();
            CreateMap<Contract.Model.IncomingCreator, Db.Model.Incoming>();
            CreateMap<Db.Model.IncomingHistory, Contract.Model.IncomingHistory>();
            CreateMap<Contract.Model.IncomingUpdater, Db.Model.Incoming>();

            CreateMap<Db.Model.Outgoing, Contract.Model.Outgoing>();
            CreateMap<Contract.Model.OutgoingCreator, Db.Model.Outgoing>();
            CreateMap<Db.Model.OutgoingHistory, Contract.Model.OutgoingHistory>();
            CreateMap<Contract.Model.OutgoingUpdater, Db.Model.Outgoing>();

            CreateMap<Db.Model.Reserve, Contract.Model.Reserve>();
            CreateMap<Contract.Model.ReserveCreator, Db.Model.Reserve>();
            CreateMap<Db.Model.ReserveHistory, Contract.Model.ReserveHistory>();
            CreateMap<Contract.Model.ReserveUpdater, Db.Model.Reserve>();

            CreateMap<Db.Model.Correction, Contract.Model.Correction>();
            CreateMap<Contract.Model.CorrectionCreator, Db.Model.Correction>();
            CreateMap<Db.Model.CorrectionHistory, Contract.Model.CorrectionHistory>();
            CreateMap<Contract.Model.CorrectionUpdater, Db.Model.Correction>();
        }
    }
}
