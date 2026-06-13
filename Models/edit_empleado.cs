using System.Collections.Generic;
using System.Web.Mvc;

namespace chaves_dayron_proyecto2_3101.Models
{
    /*
     Modelo creado unicamente para editar el empleado.
     Necesario dado el gran numero de modelos que se usan en una sola vista.

     El metodo de Tupla no sirve, ya que Razor solo permite el acceso hasta
     un maximo de cuatro items (cuando Tupla admite hasta ocho).

     Posiblemente un bug.
     */
    public class edit_empleado
    {
        public empleado empleado { get; set; }

        public persona persona { get; set; }

        public List<permiso_empleado> permiso_empleado { get; set; }

        public List<rol_empleado> rol_empleado { get; set; }

        public List<rol> rol { get; set; }

        public List<permiso> permiso { get; set; }

        public List<SelectListItem> puesto { get; set; }

        public List<SelectListItem> departamento { get; set; }

        public permiso_auxiliar permiso_auxiliar { get; set; }

        public rol_auxiliar rol_auxiliar { get; set; }

    }
}