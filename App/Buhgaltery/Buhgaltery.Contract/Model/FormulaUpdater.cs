using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    public class FormulaUpdater : IEntity
    {
        public Guid Id { get; set; }
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckNameEdit", "Formula", ErrorMessage = "Name is not valid.", AdditionalFields = "Id")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

}
