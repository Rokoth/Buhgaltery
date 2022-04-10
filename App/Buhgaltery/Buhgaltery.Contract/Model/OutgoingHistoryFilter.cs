using System;

namespace Buhgaltery.Contract.Model
{
    public class OutgoingHistoryFilter : Filter<OutgoingHistory>
    {
        public OutgoingHistoryFilter(Guid? id, int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }
}
