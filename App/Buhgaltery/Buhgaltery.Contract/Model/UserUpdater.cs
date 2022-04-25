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
        
        [Display(Name = "Только листовые элементы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public bool LeafOnly { get; set; }        
        public DateTimeOffset LastAddedDate { get; set; }
    }
}
