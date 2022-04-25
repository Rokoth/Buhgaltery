using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("h_reserve")]
    public class ReserveHistory : EntityHistory
    {
        [ColumnName("product_id")]
        public Guid ProductId { get; set; }
        [ColumnName("value")]
        public decimal Value { get; set; }
        [ColumnName("userid")]
        public Guid UserId { get; set; }
    }
}