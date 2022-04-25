﻿using System;

namespace Buhgaltery.Contract.Model
{
    public class ReserveHistory : EntityHistory
    {       
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }
        public Guid UserId { get; set; }
    }
}
