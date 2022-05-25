using System;

namespace Buhgaltery.Contract.Model
{
    public class Correction : Entity
    {       
        public Guid UserId { get; set; }      
        public string Description { get; set; }       
        public decimal Value { get; set; }        
        public DateTime CorrectionDate { get; set; }
    }
}
