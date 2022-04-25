
using System;

namespace Buhgaltery.Contract.Model
{
    public class OutgoingFilter : Filter<Outgoing>
    {
        public OutgoingFilter(int? size, int? page, string sort, Guid userId, string description, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, Guid? productId) : base(size, page, sort)
        {
            UserId = userId;
            Description = description;
            DateFrom = dateFrom;
            DateTo = dateTo;
            ProductId = productId;
        }
        public Guid UserId { get; }

        public string Description { get; set; }
        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public Guid? ProductId { get; }
    }
}
