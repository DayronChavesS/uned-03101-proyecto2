using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class nomina_sumatoria
    {
        [Display(Name = "Base Total")]
        public decimal? _base { get; set; }

        [Display(Name = "Bonificacion Total")]
        public decimal? bonificacion { get; set; }

        [Display(Name = "Deduccion Total")]
        public decimal? deduccion { get; set; }

        [Display(Name = "Final Total")]
        public decimal? final { get; set; }
    }
}