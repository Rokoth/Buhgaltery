using System;

namespace Buhgaltery.Contract.Model
{
    public class Incoming : Entity
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime IncomingDate { get; set; }
    }

    public class Outgoing : Entity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime OutgoingDate { get; set; }
        
    }
}
