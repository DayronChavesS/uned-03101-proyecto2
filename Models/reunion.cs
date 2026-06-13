using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class reunion
    {
        public int reunion_id { get; set; }
        [Display(Name = "Con el empleado...")]
        public int empleado_id { get; set; }
        [Required]
        [Display(Name = "El dia...")]
        public DateTime fecha { get; set; }
        [Required]
        [Display(Name = "Desde las...")]
        public TimeSpan hora_inicio { get; set; }
        [Required]
        [Display(Name = "Hasta las...")]
        public TimeSpan hora_fin { get; set; }
    }
}