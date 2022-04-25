﻿using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("incoming")]
    public class Incoming : Entity
    {
        [ColumnName("userid")]
        public Guid UserId { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("value")]
        public decimal Value { get; set; }
        [ColumnName("incoming_date")]
        public DateTime IncomingDate { get; set; }
    }
}