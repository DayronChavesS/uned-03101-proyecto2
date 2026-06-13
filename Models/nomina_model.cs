using System.Collections.Generic;

namespace chaves_dayron_proyecto2_3101.Models
{
    public class nomina_model
    {
        public List<nomina_detalle> NominaDetalle { get; set; }

        public nomina_sumatoria NominaSumatoria { get; set; }
    }
}