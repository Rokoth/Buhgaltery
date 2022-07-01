
using System;

namespace Buhgaltery.Contract.Model
{
    public class CorrectionFilter : Filter<Correction>
    {
        public CorrectionFilter(int? size, int? page, string sort, string description, DateTimeOffset? dateFrom, DateTimeOffset? dateTo) : base(size, page, sort)
        {            
            Description = description;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }       
        public string Description { get; set; }
        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
    }


}
