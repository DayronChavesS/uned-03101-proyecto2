using System;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class salario
    {
        public int salario_id { get; set; }
        public int empleado_id { get; set; }
        public decimal? _base { get; set; }
        public decimal? bonificacion { get; set; }
        public decimal? deduccion { get; set; }
        public decimal? final { get; set; }
        public DateTime desde_fecha { get; set; }
        public DateTime hasta_fecha { get; set; }
    }
}