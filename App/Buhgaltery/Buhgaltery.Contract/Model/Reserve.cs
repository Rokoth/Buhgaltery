using System;

namespace Buhgaltery.Contract.Model
{
    public class Reserve : Entity
    {        
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }
        public Guid UserId { get; set; }
        public string Product { get; set; }
    }
}
