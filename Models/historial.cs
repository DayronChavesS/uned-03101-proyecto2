using System;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class historial
    {
        public int historial_id { get; set; }
        public int empleado_id { get; set; }
        public int puesto_id { get; set; }
        public int departamento_id { get; set; }
        public DateTime de_fecha { get; set; }
        public DateTime hasta_fecha { get; set; }
    }
}