using System;

namespace Buhgaltery.Contract.Model
{
    public class Incoming : Entity
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset IncomingDate { get; set; }
    }
}
