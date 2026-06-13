using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class libre
    {
        public int libre_id { get; set; }
        public int empleado_id { get; set; }
        [Required]
        [Display(Name = "Fecha de la extra")]
        public DateTime fecha { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Max. 255 Caracteres")]
        [Display(Name = "Motivo de la ausentacion")]
        [DataType(DataType.MultilineText)]
        public string motivo { get; set; }
    }
}