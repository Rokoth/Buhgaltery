using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingUpdater : IEntity
    {        
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset IncomingDate { get; set; }
        public Guid Id { get; set; }
    }

    public class CorrectionUpdater : IEntity
    {        
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset CorrectionDate { get; set; }
        public Guid Id { get; set; }
    }
}
