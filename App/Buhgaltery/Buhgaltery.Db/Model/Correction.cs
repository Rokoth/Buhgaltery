using Buhgaltery.Db.Attributes;
using System;

namespace Buhgaltery.Db.Model
{
    [TableName("correction")]
    public class Correction : Entity
    {
        [ColumnName("userid")]
        public Guid UserId { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("value")]
        public decimal Value { get; set; }
        [ColumnName("correction_date")]
        public DateTimeOffset CorrectionDate { get; set; } = DateTimeOffset.Now;
    }
}