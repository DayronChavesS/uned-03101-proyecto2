using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class porcentaje
    {
        public int porcentaje_id { get; set; }
        [Required]
        [RegularExpression(@"^\d{1,1}(\.\d{1,2})?$", ErrorMessage = "Ingrese un porcentaje en formato multiplicador. (eg. 30% = 0.3)")]
        [Range(0, 1)]
        [Display(Name = "Porcentaje de deduccion aplicado por libre")]
        public decimal? libre_deduccion { get; set; }
        [Required]
        [RegularExpression(@"^\d{1,1}(\.\d{1,2})?$", ErrorMessage = "Ingrese un porcentaje en formato multiplicador. (eg. 30% = 0.3)")]
        [Range(0, 1)]
        [Display(Name = "Porcentaje de bonificacion aplicado por extra")]
        public decimal? extra_bonificacion { get; set; }
    }
}