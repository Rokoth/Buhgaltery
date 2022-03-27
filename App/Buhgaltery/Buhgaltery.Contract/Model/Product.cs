using System;

namespace Buhgaltery.Contract.Model
{
    public class Product : Entity
    {        
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AddPeriod { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastAddDate { get; set; }
    }
}
