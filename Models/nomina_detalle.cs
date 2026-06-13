using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class nomina_detalle
    {
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Display(Name = "Departamento")]
        public string departamento { get; set; }

        [Display(Name = "Puesto")]
        public string puesto { get; set; }

        [Display(Name = "Salario Base")]
        public decimal? _base { get; set; }

        [Display(Name = "Bonificacion")]
        public decimal? bonificacion { get; set; }

        [Display(Name = "Deduccion")]
        public decimal? deduccion { get; set; }

        [Display(Name = "Salario Final")]
        public decimal? final { get; set; }
    }
}