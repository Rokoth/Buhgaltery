using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("h_incoming")]
    public class IncomingHistory : EntityHistory
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