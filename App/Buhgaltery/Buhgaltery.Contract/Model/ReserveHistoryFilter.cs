using System;

namespace Buhgaltery.Contract.Model
{
    public class ReserveHistoryFilter : Filter<ReserveHistory>
    {
        public ReserveHistoryFilter(Guid? id, int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }
}
