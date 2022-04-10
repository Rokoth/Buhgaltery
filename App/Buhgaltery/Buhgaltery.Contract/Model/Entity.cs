using System;
using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    /// <summary>
    /// Базовый класс
    /// </summary>
    public class Entity : IEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Display(Name = "Идентификатор")]
        public Guid Id { get; set; }
    }
}