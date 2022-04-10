﻿using System;

namespace Buhgaltery.Contract.Model
{
    public class OutgoingUpdater : IEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTime OutgoingDate { get; set; }
        public Guid Id { get; set; }
    }
}
