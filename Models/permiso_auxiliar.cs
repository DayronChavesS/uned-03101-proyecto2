using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class permiso_auxiliar
    {
        [Display(Name = "Acceso Total")]
        public bool hasAccesoTotal { get; set; }
        [Display(Name = "Ejecucion")]
        public bool hasEjecucion { get; set; }
        [Display(Name = "Escritura")]
        public bool hasEscritura { get; set; }
        [Display(Name = "Lectura")]
        public bool hasLectura { get; set; }
        [Display(Name = "Ninguno")]
        public bool hasNinguno { get; set; }

    }
}