using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class list_turnos
    {
        [Display(Name = "ID")]
        public int empleado_id { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Display(Name = "Ingreso")]
        public TimeSpan tiempo_inicio { get; set; }

        [Display(Name = "Salida")]
        public TimeSpan tiempo_fin { get; set; }
    }
}