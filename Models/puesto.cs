using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class puesto
    {
        public int puesto_id { get; set; }
        [Display(Name = "Puesto")]
        public string descripcion { get; set; }
        [Required]
        [Display(Name = "Salario")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Solo se admite una precision decimal de 2 digitos.")]
        [Range(0, Int32.MaxValue)]
        public decimal? salario { get; set; }
    }
}