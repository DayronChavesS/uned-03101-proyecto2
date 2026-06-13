using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class turno
    {
        public int turno_id { get; set; }
        public int empleado_id { get; set; }
        [Required]
        [Display(Name = "Hora de ingreso")]
        public TimeSpan tiempo_inicio { get; set; }
        [Required]
        [Display(Name = "Hora de salida")]
        public TimeSpan tiempo_fin { get; set; }
    }
}