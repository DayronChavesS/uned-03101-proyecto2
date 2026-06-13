using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class evaluacion
    {
        public int evaluacion_id { get; set; }
        public int reunion_id { get; set; }
        public int empleado_id { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Max. 255 Caracteres")]
        [Display(Name = "Objetivo")]
        [DataType(DataType.MultilineText)]
        public string objetivo { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Max. 255 Caracteres")]
        [Display(Name = "Seguimiento")]
        [DataType(DataType.MultilineText)]
        public string seguimiento { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Max. 255 Caracteres")]
        [Display(Name = "Retroalimentacion")]
        [DataType(DataType.MultilineText)]
        public string retroalimentacion { get; set; }
    }
}