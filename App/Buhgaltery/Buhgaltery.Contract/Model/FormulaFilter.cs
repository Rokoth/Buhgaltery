
using System;

namespace Buhgaltery.Contract.Model
{
    /// <summary>
    /// filter for formula entity
    /// </summary>
    public class FormulaFilter : Filter<Formula>
    {
        public FormulaFilter(int? size, int? page, string sort, string name, bool? isDefault) : base(size, page, sort)
        {
            Name = name;
            IsDefault = isDefault;
        }
        public string Name { get; }
        public bool? IsDefault { get; }
    }

    public class IncomingFilter : Filter<Incoming>
    {
        public IncomingFilter(int? size, int? page, string sort, Guid userId) : base(size, page, sort)
        {
            UserId = userId;
        }
        public Guid UserId { get; }
    }

    public class OutgoingFilter : Filter<Outgoing>
    {
        public OutgoingFilter(int? size, int? page, string sort, Guid userId) : base(size, page, sort)
        {
            UserId = userId;
        }
        public Guid UserId { get; }
    }


    


}
