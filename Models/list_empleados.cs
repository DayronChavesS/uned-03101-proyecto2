using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class list_empleados
    {
        [Display(Name = "ID")]
        public int empleado_id { get; set; }

        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Display(Name = "Apellido")]
        public string apellido { get; set; }

        [Display(Name = "Departamento")]
        public string departamento { get; set; }

        [Display(Name = "Puesto")]
        public string puesto { get; set; }
    }
}