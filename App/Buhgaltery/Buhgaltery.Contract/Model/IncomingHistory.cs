using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingHistory: EntityHistory
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime IncomingDate { get; set; }
    }
}
