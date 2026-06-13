using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class departamento
    {
        public int departamento_id { get; set; }
        [Display(Name = "Departamento")]
        public string descripcion { get; set; }
    }
}