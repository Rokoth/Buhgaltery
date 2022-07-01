using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingCreator
    {         
        public string Description { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset IncomingDate { get; set; }
    }

    public class CorrectionCreator
    {        
        public string Description { get; set; }
        public decimal? Value { get; set; }
        public decimal? TotalValue { get; set; }
    }
}
