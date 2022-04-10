using System;

namespace Buhgaltery.Contract.Model
{
    public class IncomingHistoryFilter : Filter<IncomingHistory>
    {
        public IncomingHistoryFilter(Guid? id, int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }
}
