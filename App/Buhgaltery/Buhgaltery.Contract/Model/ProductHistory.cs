﻿using System;

namespace Buhgaltery.Contract.Model
{
    public class ProductHistory : EntityHistory
    {
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int AddPeriod { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastAddDate { get; set; }
    }
}
