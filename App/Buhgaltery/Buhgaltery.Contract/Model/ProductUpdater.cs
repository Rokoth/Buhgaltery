using System;

namespace Buhgaltery.Contract.Model
{
    public class ProductUpdater : IEntity
    {        
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int AddPeriod { get; set; }
        public Guid Id { get; set; }
    }
}
