using System;

namespace Buhgaltery.Contract.Model
{
    public class ProductHistoryFilter : Filter<ProductHistory>
    {
        public ProductHistoryFilter(Guid? id, int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }
}
