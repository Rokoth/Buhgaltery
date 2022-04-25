namespace Buhgaltery.Contract.Model
{
    public class AllData
    {
        public decimal Incomings { get; set; }
        public decimal Outgoings { get; set; }
        public decimal Reserves { get; set; }
        public decimal Free { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
