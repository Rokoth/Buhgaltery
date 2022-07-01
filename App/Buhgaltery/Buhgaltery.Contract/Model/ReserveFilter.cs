using System;

namespace Buhgaltery.Contract.Model
{
    public class ReserveFilter : Filter<Reserve>
    {
        public ReserveFilter(int? size, int? page, string sort, Guid? productId) : base(size, page, sort)
        {
            ProductId = productId;            
        }
        /// <summary>
        /// User Name
        /// </summary>
        public Guid? ProductId { get; }       
    }
}
