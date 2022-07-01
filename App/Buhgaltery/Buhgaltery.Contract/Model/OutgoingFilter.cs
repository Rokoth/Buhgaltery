
using System;

namespace Buhgaltery.Contract.Model
{
    public class OutgoingFilter : Filter<Outgoing>
    {
        public OutgoingFilter(int? size, int? page, string sort, string description, DateTimeOffset? dateFrom, DateTimeOffset? dateTo, Guid? productId) : base(size, page, sort)
        {            
            Description = description;
            DateFrom = dateFrom;
            DateTo = dateTo;
            ProductId = productId;
        }        

        public string Description { get; set; }
        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public Guid? ProductId { get; }
    }
}
