﻿using System;

namespace Buhgaltery.Contract.Model
{
    public class ProductFilter : Filter<Product>
    {
        public ProductFilter(int? size, int? page, string sort, Guid userId, Guid? parentId, string name, bool leafOnly, DateTimeOffset? lastAddDateFrom, DateTimeOffset? lastAddDateTo) : base(size, page, sort)
        {
            ParentId = parentId;
            UserId = userId;
            Name = name;
            LeafOnly = leafOnly;
            LastAddDateFrom = lastAddDateFrom;
            LastAddDateTo = lastAddDateTo;
        }      

        public Guid? ParentId { get; }
        public Guid UserId { get; }
        public string Name { get; }
        public bool LeafOnly { get; }
        public DateTimeOffset? LastAddDateFrom { get; }
        public DateTimeOffset? LastAddDateTo { get; }
    }
}
