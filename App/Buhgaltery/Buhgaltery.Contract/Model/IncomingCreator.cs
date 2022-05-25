using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingCreator
    { 
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Sum { get; set; }
        public DateTime IncomingDate { get; set; }
    }

    public class CorrectionCreator
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal? Value { get; set; }
        public decimal? TotalValue { get; set; }
    }
}
