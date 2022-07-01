using System;

namespace Buhgaltery.Contract.Model
{
    public class Outgoing : Entity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset OutgoingDate { get; set; }        
        public string Product { get; set; }
    }
}
