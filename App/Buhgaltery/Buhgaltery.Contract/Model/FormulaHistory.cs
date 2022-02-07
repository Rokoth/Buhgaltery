using System.ComponentModel.DataAnnotations;

namespace Buhgaltery.Contract.Model
{
    public class FormulaHistory : EntityHistory
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

}
