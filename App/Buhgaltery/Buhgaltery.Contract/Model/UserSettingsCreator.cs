using System;
using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    public class UserSettingsCreator
    {
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }               
        [Display(Name = "Только листовые элементы")]
        public bool LeafOnly { get; set; }       
        [Display(Name = "Пользователь")]
        public string User { get; set; }
        [Display(Name = "Величина резерва по умолчанию")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public decimal DefaultReserveValue { get; set; }

        [Display(Name = "Период добавления элементов в резервы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public int AddPeriod { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}