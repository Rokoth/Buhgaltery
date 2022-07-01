using System;

namespace Buhgaltery.Contract.Model
{
    public class ReserveUpdater : IEntity
    {
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }        
        public Guid Id { get; set; }
    }
}
