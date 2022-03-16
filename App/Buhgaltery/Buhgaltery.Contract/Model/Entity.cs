using System;
using System.Collections.Generic;
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

    public class UserSettings : Entity
    {
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }             
        [Display(Name = "Только листовые элементы")]
        public bool LeafOnly { get; set; }  
        [Display(Name = "Пользователь")]
        public string User { get; set; }
    }

    public class UserSettingsCreator
    {
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
       
        [Display(Name = "Количество элементов (для типа по кол-ву)")]
        public int? ScheduleCount { get; set; }
        [Display(Name = "Промежуток расписания (для типа по времени)")]
        public int? ScheduleTimeSpan { get; set; } // hours       
        [Display(Name = "Время задачи по умолчанию")]
        public int DefaultProjectTimespan { get; set; }
        [Display(Name = "Только листовые элементы")]
        public bool LeafOnly { get; set; }
        [Display(Name = "Сдвиг расписания (в мин)")]
        public int ScheduleShift { get; set; }
        [Display(Name = "Пользователь")]
        public string User { get; set; }
    }

    public class UserSettingsUpdater : IEntity
    {
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }        
        [Display(Name = "Промежуток расписания (для типа по времени)")]
        public int? ScheduleTimeSpan { get; set; } // hours       
        [Display(Name = "Время задачи по умолчанию")]
        public int DefaultProjectTimespan { get; set; }
        [Display(Name = "Только листовые элементы")]
        public bool LeafOnly { get; set; }
        [Display(Name = "Сдвиг расписания (в мин)")]
        public int ScheduleShift { get; set; }
        [Display(Name = "Пользователь")]
        public string User { get; set; }
        [Display(Name = "ИД")]
        public Guid Id { get; set; }
    }

    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> data, int allCount)
        {
            Data = data;
            PageCount = allCount;
        }
        public IEnumerable<T> Data { get; }
        public int PageCount { get; }
    }

}