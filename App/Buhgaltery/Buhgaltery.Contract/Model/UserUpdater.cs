using System;
using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    public class UserUpdater: IEntity
    {
        public Guid Id { get; set; }
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool PasswordChanged { get; set; }

        [Display(Name = "ИД формулы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid FormulaId { get; set; }

        [Display(Name = "Формула")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Formula { get; set; }
       
        [Display(Name = "Количество элементов (для типа по кол-ву)")]
        public int? ScheduleCount { get; set; }
        [Display(Name = "Промежуток расписания (для типа по времени)")]
        public int? ScheduleTimeSpan { get; set; } // hours       
        [Display(Name = "Время задачи по умолчанию")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public int DefaultProjectTimespan { get; set; }
        [Display(Name = "Только листовые элементы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public bool LeafOnly { get; set; }
        [Display(Name = "Сдвиг расписания (в мин)")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public int ScheduleShift { get; set; }
        public DateTimeOffset LastAddedDate { get; set; }
    }


    
    public class Reserve : Entity
    {        
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }
        public Guid UserId { get; set; }
    }

    public class ReserveFilter : Filter<Reserve>
    {
        public ReserveFilter(int? size, int? page, string sort, Guid userId, Guid? productId) : base(size, page, sort)
        {
            ProductId = productId;
            UserId = userId;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public Guid? ProductId { get; }
        public Guid UserId { get; }
    }

    

    public class ReserveCreator
    {
        public Guid? ProductId { get; set; }
        public decimal? Value { get; set; }
        public Guid UserId { get; set; }
    }

    public class ReserveUpdater : IEntity
    {
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }

    public class ReserveHistory : EntityHistory
    {       
        public Guid ProductId { get; set; }
        public decimal Value { get; set; }
        public Guid UserId { get; set; }
    }

    public class ProductFilter : Filter<Product>
    {
        public ProductFilter(int? size, int? page, string sort, Guid userId, Guid? parentId, string name, bool leafOnly, DateTimeOffset? lastAddDate) : base(size, page, sort)
        {
            ParentId = parentId;
            UserId = userId;
            Name = name;
            LeafOnly = leafOnly;
            LastAddDate = lastAddDate;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public Guid? ParentId { get; }
        public Guid UserId { get; }
        public string Name { get; }
        public bool LeafOnly { get; }
        public DateTimeOffset? LastAddDate { get; }
    }



    public class ProductCreator
    {
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AddPeriod { get; set; }       
    }

    public class ProductUpdater : IEntity
    {
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AddPeriod { get; set; }
        public Guid Id { get; set; }
    }

    public class ProductHistory : EntityHistory
    {
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AddPeriod { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastAddDate { get; set; }
    }

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
