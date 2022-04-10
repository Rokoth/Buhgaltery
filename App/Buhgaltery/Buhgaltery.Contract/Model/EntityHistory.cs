using System;

namespace Buhgaltery.Contract.Model
{
    public class EntityHistory : Entity
    {
        public long HId { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
        public bool IsDeleted { get; set; }
    }

}