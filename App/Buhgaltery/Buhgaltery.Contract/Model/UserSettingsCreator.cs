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
    }

}