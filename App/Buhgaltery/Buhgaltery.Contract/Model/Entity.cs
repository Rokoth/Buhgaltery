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

    public class EntityHistory : Entity
    {
        public long HId { get; set; }
        public DateTimeOffset ChangeDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ErrorMessage
    { 
        public string Message { get; set; }
        public string Source { get; set; }
    }

    public class ClientIdentityResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
    }


    public class UserIdentity : IIdentity
    {
        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}