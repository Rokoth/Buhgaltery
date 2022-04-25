using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("h_outgoing")]
    public class OutgoingHistory : Entity
    {
        [ColumnName("userid")]
        public Guid UserId { get; set; }
        [ColumnName("product_id")]
        public Guid ProductId { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("value")]
        public decimal Value { get; set; }
        [ColumnName("outgoing_date")]
        public DateTime OutgoingDate { get; set; }
    }
}