using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingCreator
    { 
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Sum { get; set; }
        public DateTime IncomingDate { get; set; }
    }
}
