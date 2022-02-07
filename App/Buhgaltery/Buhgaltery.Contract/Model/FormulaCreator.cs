using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    public class FormulaCreator
    {
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckName", "Formula", ErrorMessage = "Name is not valid.")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

}
