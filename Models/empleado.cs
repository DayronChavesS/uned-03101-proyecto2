using System;
using System.ComponentModel.DataAnnotations;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class empleado
    {
        [Display(Name = "ID")]
        public int empleado_id { get; set; }
        public int persona_id { get; set; }
        [Display(Name = "Departamento")]
        public int departamento_id { get; set; }
        [Display(Name = "Puesto")]
        public int puesto_id { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Ingrese solamente la fecha")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime desde_fecha { get; set; }
    }
}