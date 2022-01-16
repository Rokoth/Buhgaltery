using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("formula")]
    public class Formula : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("text")]
        public string Text { get; set; }

        [ColumnName("is_default")]
        public bool IsDefault { get; set; }
    }

    [TableName("reserve")]
    public class Reserve : Entity
    {
        [ColumnName("product_id")]
        public Guid ProductId { get; set; }
        [ColumnName("value")]
        public decimal Value { get; set; }
        [ColumnName("userid")]
        public Guid UserId { get; set; }
    }

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