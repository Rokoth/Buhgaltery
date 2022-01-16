using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Buhgaltery.Contract.Model
{
    public class User : Entity
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckName", "User", ErrorMessage = "Имя уже используется")]
        public string Name { get; set; }

        [Display(Name = "Описание")]       
        public string Description { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckLogin", "User", ErrorMessage = "Логин уже используется")]
        public string Login { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        public bool LeafOnly { get; set; }
        public Guid FormulaId { get; set; }
        public string Formula { get; set; }
        public DateTimeOffset? LastAddedDate { get; set; }
        public int AddPeriod { get; set; }
    }
}
