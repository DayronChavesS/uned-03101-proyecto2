using chaves_dayron_proyecto2_3101.DataBase;
using chaves_dayron_proyecto2_3101.Misc;
using chaves_dayron_proyecto2_3101.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace chaves_dayron_proyecto2_3101.Controllers
{
    public class HomeController : Controller
    {
        DBCommand dbcmd = new DBCommand();
        Notify mailcmd = new Notify();
        ExceptionManager exmng = new ExceptionManager();

        Dictionary<string, object> theData = new Dictionary<string, object>();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            //notificaciones para el usuario
            if (TempData["success"] != null)
            {
                ViewData["success"] = TempData["success"];
                TempData["success"] = null;
            }

            if (TempData["failure"] != null)
            {
                ViewData["failure"] = TempData["failure"];
                TempData["failure"] = null;
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddEmpleado1()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddEmpleado1(persona nuevaPersona)
        {
            try
            {
                theData.Add("persona", nuevaPersona);

                //cedula duplicada
                object[] result = dbcmd.cmdTypeHasRows(1, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(result);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                if ((int)result[0] == 0)
                {
                    ModelState.AddModelError("CedulaExist", "La cedula ingresada ya existe.");
                    return View("AddEmpleado1", nuevaPersona);
                }

                //correo duplicado
                object[] result2 = dbcmd.cmdTypeHasRows(2, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(result2);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                if ((int)result2[0] == 0)
                {
                    ModelState.AddModelError("CorreoExist", "El correo ingresado ya existe.");
                    return View("AddEmpleado1", nuevaPersona);
                }

                return RedirectToAction("AddEmpleado2", nuevaPersona);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddEmpleado2(persona nuevaPersona)
        {
            try
            {
                //obtenemos la lista de departamentos
                List<object> objDept = dbcmd.cmdTypeReader(1, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(objDept);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la lista de puestos
                List<object> objPts = dbcmd.cmdTypeReader(15, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(objPts);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos a los modelos de datos reales
                List<departamento> departamentos = objDept.OfType<departamento>().ToList();
                List<puesto> puestos = objPts.OfType<puesto>().ToList();

                //creamos los datos de los inputs del formulario
                List<SelectListItem> puestosSelect = new List<SelectListItem>();
                foreach (puesto obj in puestos)
                {
                    puestosSelect.Add(new SelectListItem
                    {
                        Text = obj.descripcion,
                        Value = obj.puesto_id.ToString()
                    });
                }

                List<SelectListItem> departamentoSelect = new List<SelectListItem>();
                foreach (departamento obj in departamentos)
                {
                    departamentoSelect.Add(new SelectListItem
                    {
                        Text = obj.descripcion,
                        Value = obj.departamento_id.ToString()
                    });
                }

                //metodo para enviar varios tipos de datos a las vistas.
                var tupleModel = new Tuple<empleado, persona, List<SelectListItem>, List<SelectListItem>>(new empleado(), nuevaPersona, puestosSelect, departamentoSelect);
                return View(tupleModel);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //cuando se usan tuplas como modelos, se debe especificar explicitamente el ModelBinding para submits
        public ActionResult AddEmpleado2([Bind(Prefix = "Item1")] empleado nuevoEmpleado, [Bind(Prefix = "Item2")] persona nuevaPersona)
        {
            try
            {
                theData.Add("persona", nuevaPersona);

                //insertamos la persona
                object[] resultInsertPersona = dbcmd.cmdTypeNonQuery(12, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertPersona);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos el id_persona
                List<object> resultGetPersonaID = dbcmd.cmdTypeReader(25, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetPersonaID);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //ingresamos los datos restantes del empleado
                nuevaPersona = (persona)resultGetPersonaID[0];
                nuevoEmpleado.persona_id = nuevaPersona.persona_id;
                nuevoEmpleado.desde_fecha = DateTime.Parse(DateTime.Today.ToString("yyyy-MM-dd"));
                theData.Add("empleado", nuevoEmpleado);

                //insertamos el empleado
                object[] resultInsertEmpleado = dbcmd.cmdTypeNonQuery(6, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEmpleado);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //actualizamos el empleado para recuperar la id
                List<object> resultGetEmpleadoWithPersona = dbcmd.cmdTypeReader(32, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetEmpleadoWithPersona);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Remove("empleado");
                nuevoEmpleado = (empleado)resultGetEmpleadoWithPersona[0];
                theData.Add("empleado", nuevoEmpleado);

                //por defecto, todos los empleados adquieren el turno diurno
                turno turno = new turno()
                {
                    empleado_id = nuevoEmpleado.empleado_id,
                    tiempo_inicio = new TimeSpan(8, 0, 0),
                    tiempo_fin = new TimeSpan(14, 0, 0)
                };

                theData.Add("turno", turno);

                //por defecto, todos los nuevos empleados tienen el rol invitado
                rol_empleado rol_empleado = new rol_empleado()
                {
                    empleado_id = nuevoEmpleado.empleado_id,
                    rol_id = 5
                };

                theData.Add("rol_empleado", rol_empleado);

                //por defecto, todos los nuevos empleados no tienen permisos
                permiso_empleado permiso_empleado = new permiso_empleado()
                {
                    empleado_id = nuevoEmpleado.empleado_id,
                    permiso_id = 5
                };

                theData.Add("permiso_empleado", permiso_empleado);

                //por defecto, se asigna el deposito del salario del mes actual
                DateTime now = DateTime.Now;
                salario salario = new salario()
                {
                    empleado_id = nuevoEmpleado.empleado_id,
                    desde_fecha = new DateTime(now.Year, now.Month, 1),
                };
                salario.hasta_fecha = salario.desde_fecha.AddMonths(1).AddDays(-1);
                theData.Add("salario", salario);

                //insertamos el empleado en la tabla de salarios
                object[] resultInsertEmpleadoSalario = dbcmd.cmdTypeNonQuery(15, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEmpleadoSalario);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //insertamos el empleado en la tabla de turnos
                object[] resultInsertEmpleadoTurno = dbcmd.cmdTypeNonQuery(16, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEmpleadoTurno);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //insertamos el empleado en la tabla rol_empleado
                object[] resultInsertEmpleadoRol = dbcmd.cmdTypeNonQuery(14, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEmpleadoRol);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //insertamos el empleado en la tabla permiso_empleado
                object[] resultInsertEmpleadoPermiso = dbcmd.cmdTypeNonQuery(11, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEmpleadoPermiso);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //notificamos al empleado
                //obtenemos descripciones
                List<object> resultGetDepartamento = dbcmd.cmdTypeReader(26, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetDepartamento);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });
                List<object> resultGetPuesto = dbcmd.cmdTypeReader(27, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetPuesto);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                departamento departamento = (departamento)resultGetDepartamento[0];
                puesto puesto = (puesto)resultGetPuesto[0];
                theData.Add("departamento", departamento);
                theData.Add("puesto", puesto);

                //enviamos el correo
                object[] resultSendMail = mailcmd.SendEmail(1, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "El empleado se ha agregado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ListEmpleados()
        {
            try
            {
                //obtenemos una lista de empleados (vista personalizada)
                List<object> resultGetEmpleados = dbcmd.cmdTypeReader(30, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetEmpleados);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos a los modelos de datos reales
                List<list_empleados> empleados = resultGetEmpleados.OfType<list_empleados>().ToList();

                return View(empleados);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ListEmpleados(int empleado_id)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EditEmpleado(int empleado_id)
        {
            try
            {
                empleado empleado = new empleado();
                persona persona = new persona();

                empleado.empleado_id = empleado_id;
                theData.Add("empleado", empleado);

                //obtenemos el empleado
                List<object> getEmpleadoResult = dbcmd.cmdTypeReader(2, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getEmpleadoResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData["empleado"] = (empleado)getEmpleadoResult[0];

                //obtenemos la persona asociada al empleado
                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos los permisos asignados actualmente al empleado
                List<object> getPermisoEmpleadoResult = dbcmd.cmdTypeReader(11, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPermisoEmpleadoResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos los roles asignados actualmente empleado
                List<object> getRolEmpleadoResult = dbcmd.cmdTypeReader(18, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getRolEmpleadoResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la lista de permisos
                List<object> getPermisosResult = dbcmd.cmdTypeReader(12, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPermisosResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la lista de roles
                List<object> getRolesResult = dbcmd.cmdTypeReader(19, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getRolesResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la lista de departamentos
                List<object> getDepartamentosResult = dbcmd.cmdTypeReader(1, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getDepartamentosResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la lista de puestos
                List<object> getPuestosResult = dbcmd.cmdTypeReader(15, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPuestosResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos a los modelos de datos reales
                empleado = (empleado)getEmpleadoResult[0];
                persona = (persona)getPersonaResult[0];
                List<permiso_empleado> permisosDelEmpleado = getPermisoEmpleadoResult.OfType<permiso_empleado>().ToList();
                List<rol_empleado> rolesDelEmpleado = getRolEmpleadoResult.OfType<rol_empleado>().ToList();
                List<rol> roles = getRolesResult.OfType<rol>().ToList();
                List<permiso> permisos = getPermisosResult.OfType<permiso>().ToList();
                List<departamento> departamentos = getDepartamentosResult.OfType<departamento>().ToList();
                List<puesto> puestos = getPuestosResult.OfType<puesto>().ToList();
                permiso_auxiliar permisoAux = (permiso_auxiliar)ProcessPermisoAuxiliar(permisosDelEmpleado, null, empleado.empleado_id)["permiso_auxiliar"];
                rol_auxiliar rolAux = (rol_auxiliar)ProcessRolAuxiliar(rolesDelEmpleado, null, empleado.empleado_id)["rol_auxiliar"];

                //creamos los datos para los dropDownList
                List<SelectListItem> puestosSelect = new List<SelectListItem>();
                foreach (puesto obj in puestos)
                {
                    puestosSelect.Add(new SelectListItem
                    {
                        Text = obj.descripcion,
                        Value = obj.puesto_id.ToString()
                    });
                }

                List<SelectListItem> departamentoSelect = new List<SelectListItem>();
                foreach (departamento obj in departamentos)
                {
                    departamentoSelect.Add(new SelectListItem
                    {
                        Text = obj.descripcion,
                        Value = obj.departamento_id.ToString()
                    });
                }

                //enviamos toda la informacion a la vista con un modelo especializado
                edit_empleado editEmpleado = new edit_empleado()
                {
                    empleado = empleado,
                    persona = persona,
                    permiso_empleado = permisosDelEmpleado,
                    rol_empleado = rolesDelEmpleado,
                    rol = roles,
                    permiso = permisos,
                    puesto = puestosSelect,
                    departamento = departamentoSelect,
                    permiso_auxiliar = permisoAux,
                    rol_auxiliar = rolAux
                };

                return View(editEmpleado);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditEmpleado(edit_empleado newData)
        {
            try
            {
                newData.empleado.desde_fecha = DateTime.Now;

                //convertimos los auxiliares de vista en datos para db
                List<permiso_empleado> permisoEmpleado = (List<permiso_empleado>)ProcessPermisoAuxiliar(null, newData.permiso_auxiliar, newData.empleado.empleado_id)["permiso_empleado"];
                List<rol_empleado> rolEmpleado = (List<rol_empleado>)ProcessRolAuxiliar(null, newData.rol_auxiliar, newData.empleado.empleado_id)["rol_empleado"];

                theData.Add("persona", newData.persona);
                theData.Add("empleado", newData.empleado);
                theData.Add("rol_empleado", rolEmpleado[0]);
                theData.Add("permiso_empleado", permisoEmpleado[0]); //<- insercion temporal

                //actualizamos la persona
                object[] resultUpdatePersona = dbcmd.cmdTypeNonQuery(18, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultUpdatePersona);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //actualizamos el empleado
                object[] resultUpdateEmpleado = dbcmd.cmdTypeNonQuery(17, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultUpdateEmpleado);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //elminamos los permisos viejos del empleado
                object[] resultDeletePermisoEmpleado = dbcmd.cmdTypeNonQuery(3, theData);
                theData.Remove("permiso_empleado");
                TempData["failure"] = exmng.exceptionCheckerForArray(resultDeletePermisoEmpleado);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //eliminamos el rol viejo del empleado
                object[] resultDeleteRolEmpleado = dbcmd.cmdTypeNonQuery(4, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultDeleteRolEmpleado);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //insertamos el nuevo rol
                object[] resultInsertRol = dbcmd.cmdTypeNonQuery(14, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertRol);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //insertamos los nuevos permisos
                //pueden ser varios
                permisoEmpleado.ForEach(delegate (permiso_empleado item)
                {
                    theData.Add("permiso_empleado", item);
                    object[] resultInsertPermiso = dbcmd.cmdTypeNonQuery(11, theData);
                    theData.Remove("permiso_empleado");
                    TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertPermiso);
                });
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });


                //obtenemos la descripcion del permiso(s)
                int i = 0;
                permisoEmpleado.ForEach(delegate (permiso_empleado item)
                {
                    theData.Add("permiso",
                        new permiso()
                        {
                            permiso_id = item.permiso_id
                        }
                    );
                    List<object> getPermisoResult = dbcmd.cmdTypeReader(28, theData);
                    theData.Remove("permiso");

                    i++;
                    theData.Add("permiso" + i, new permiso()
                    {
                        permiso_id = item.permiso_id,
                        descripcion = (string)getPermisoResult[0].GetType().GetProperty("descripcion").GetValue(getPermisoResult[0], null)
                    });

                    TempData["failure"] = exmng.exceptionCheckerForList(getPermisoResult);
                });
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos descripcion del rol
                theData.Add("rol", new rol()
                {
                    rol_id = rolEmpleado[0].rol_id
                });

                List<object> getRolResult = dbcmd.cmdTypeReader(29, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getRolResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Remove("rol");

                theData.Add("rol", new rol()
                {
                    rol_id = rolEmpleado[0].rol_id,
                    descripcion = (string)getRolResult[0].GetType().GetProperty("descripcion").GetValue(getRolResult[0], null)
                });


                //obtenemos descripcion de departamento
                List<object> getDepartamentoResult = dbcmd.cmdTypeReader(26, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getDepartamentoResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("departamento", new departamento()
                {
                    departamento_id = newData.empleado.departamento_id,
                    descripcion = (string)getDepartamentoResult[0].GetType().GetProperty("descripcion").GetValue(getDepartamentoResult[0], null)
                });


                //obtenemos descripcion de puesto
                List<object> getPuestoResult = dbcmd.cmdTypeReader(27, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPuestoResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("puesto", new puesto()
                {
                    puesto_id = newData.empleado.puesto_id,
                    descripcion = (string)getPuestoResult[0].GetType().GetProperty("descripcion").GetValue(getPuestoResult[0], null),
                    salario = (decimal)getPuestoResult[0].GetType().GetProperty("salario").GetValue(getPuestoResult[0], null)
                });


                //enviamos los correo electronicos
                object[] resultSendMail = mailcmd.SendEmail(2, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                object[] resultSendMail2 = mailcmd.SendEmail(3, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail2);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "El empleado se ha actualizado.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Nomina()
        {
            try
            {
                //se obtiene la nomina
                List<object> resultGetDetalleNomina = dbcmd.cmdTypeReader(10, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetDetalleNomina);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //se calcula la sumatoria
                List<object> resultGetSumatoriaNomina = dbcmd.cmdTypeReader(22, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetSumatoriaNomina);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos a los modelos de datos reales
                List<nomina_detalle> detalle = resultGetDetalleNomina.OfType<nomina_detalle>().ToList();
                nomina_sumatoria sumatoria = (nomina_sumatoria)resultGetSumatoriaNomina[0];

                //se crea el modelo especializado para la vista
                nomina_model model = new nomina_model()
                {
                    NominaDetalle = detalle,
                    NominaSumatoria = sumatoria
                };


                return View(model);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Salario()
        {
            try
            {
                //Obtener una lista de salarios y puestos
                List<object> resultGetSalarios = dbcmd.cmdTypeReader(15, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetSalarios);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //Obtener los porcentajes
                List<object> resultGetPorcentajes = dbcmd.cmdTypeReader(31, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetSalarios);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos
                List<puesto> puestos = resultGetSalarios.OfType<puesto>().ToList();
                porcentaje porcentaje = (porcentaje)resultGetPorcentajes[0];

                //enviamos los datos en forma de tupla

                //se envian los datos
                var tupleModel = new Tuple<List<puesto>, porcentaje>(puestos, porcentaje);
                return View(tupleModel);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Salario([Bind(Prefix = "Item1")] List<puesto> newSalarios, [Bind(Prefix = "Item2")] porcentaje newPorcentaje)
        {
            try
            {
                //actualizamos los salarios
                foreach (puesto obj in newSalarios)
                {
                    theData.Add("puesto", obj);

                    object[] resultUpdateSalario = dbcmd.cmdTypeNonQuery(20, theData);
                    TempData["failure"] = exmng.exceptionCheckerForArray(resultUpdateSalario);
                    if (TempData["failure"] != null)
                        return RedirectToAction("Index", "Home", new { });

                    theData.Remove("puesto");
                }

                //actualizamos el porcentaje
                theData.Add("porcentaje", newPorcentaje);

                object[] resultUpdatePorcentaje = dbcmd.cmdTypeNonQuery(21, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultUpdatePorcentaje);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //redirijimos el usuario a la Nomina
                return RedirectToAction("Nomina", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CrearReunion()
        {
            try
            {
                List<object> resultGetEmpleados = dbcmd.cmdTypeReader(3, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetEmpleados);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                List<object> resultGetPersonas = dbcmd.cmdTypeReader(14, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetPersonas);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                List<SelectListItem> empleadosSelect = new List<SelectListItem>();
                foreach (empleado objEmpleado in resultGetEmpleados)
                {
                    foreach (persona objPersona in resultGetPersonas)
                    {
                        if (objEmpleado.persona_id == objPersona.persona_id)
                            empleadosSelect.Add(new SelectListItem
                            {
                                Text = objPersona.nombre + " " + objPersona.apellido,
                                Value = objEmpleado.empleado_id.ToString()
                            });
                    }
                }
                var tupleModel = new Tuple<List<SelectListItem>, reunion>(empleadosSelect, new reunion());
                return View(tupleModel);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CrearReunion([Bind(Prefix = "Item1")] List<SelectListItem> empleadosSelect, [Bind(Prefix = "Item2")] reunion newReunion)
        {
            try
            {
                if (newReunion.hora_inicio > newReunion.hora_fin)
                {
                    ModelState.AddModelError("HoraInicio", "La hora de inicio esta despues de la hora de fin.");
                    return View(new Tuple<List<SelectListItem>, reunion>(empleadosSelect, newReunion));
                }

                //insertar la reunion
                theData.Add("reunion", newReunion);
                object[] resultInsertNewReunion = dbcmd.cmdTypeNonQuery(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertNewReunion);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la persona asociada al empleado
                theData.Add("empleado", new empleado() { empleado_id = newReunion.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("persona", (persona)getPersonaResult[0]);

                //enviamos la notificacion al empleado
                object[] resultSendMail = mailcmd.SendEmail(7, theData);

                TempData["success"] = "La reunion se ha agendado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult NewEvaluacion1()
        {
            try
            {
                //obtener lista de empleados
                List<object> resultGetEmpleados = dbcmd.cmdTypeReader(3, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetEmpleados);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                List<object> resultGetPersonas = dbcmd.cmdTypeReader(14, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetPersonas);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                List<SelectListItem> empleadosSelect = new List<SelectListItem>();
                foreach (empleado objEmpleado in resultGetEmpleados)
                {
                    foreach (persona objPersona in resultGetPersonas)
                    {
                        if (objEmpleado.persona_id == objPersona.persona_id)
                            empleadosSelect.Add(new SelectListItem
                            {
                                Text = objPersona.nombre + " " + objPersona.apellido,
                                Value = objEmpleado.empleado_id.ToString()
                            });
                    }
                }

                var tupleModel = new Tuple<List<SelectListItem>, int>(empleadosSelect, new int());
                return View(tupleModel);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult NewEvaluacion1([Bind(Prefix = "Item2")] int empleado_id)
        {
            try
            {
                return RedirectToAction("NewEvaluacion2", "Home", new { empleado_id });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult NewEvaluacion2(int empleado_id)
        {
            try
            {
                //obtener lista de reuniones del empleado
                theData.Add("empleado", new empleado() { empleado_id = empleado_id });
                List<object> resultGetReunionEmpleado = dbcmd.cmdTypeReader(16, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetReunionEmpleado);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                List<SelectListItem> reunionesSelect = new List<SelectListItem>();
                foreach (reunion objReunion in resultGetReunionEmpleado)
                {
                    reunionesSelect.Add(new SelectListItem
                    {
                        Text = "F:" + objReunion.fecha.ToString() + " H:" + objReunion.hora_inicio.ToString(),
                        Value = objReunion.reunion_id.ToString()
                    });
                }

                var tupleModel = new Tuple<List<SelectListItem>, int, int>(reunionesSelect, empleado_id, new int());
                return View(tupleModel);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult NewEvaluacion2([Bind(Prefix = "Item2")] int empleado_id, [Bind(Prefix = "Item3")] int reunion_id)
        {
            try
            {
                return RedirectToAction("NewEvaluacion3", "Home", new { empleado_id, reunion_id });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult NewEvaluacion3(int empleado_id, int reunion_id)
        {
            try
            {
                evaluacion evaluacion = new evaluacion()
                {
                    reunion_id = reunion_id,
                    empleado_id = empleado_id
                };

                return View(evaluacion);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult NewEvaluacion3(evaluacion evaluacion)
        {
            try
            {
                //guardamos la nueva evaluacion
                theData.Add("evaluacion", evaluacion);
                object[] resultInsertEvaluacion = dbcmd.cmdTypeNonQuery(7, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertEvaluacion);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la persona asociada al empleado
                theData.Add("empleado", new empleado() { empleado_id = evaluacion.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("persona", (persona)getPersonaResult[0]);

                //informamos al empleado
                object[] resultSendMail = mailcmd.SendEmail(8, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "La evaluacion se ha realizado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Horario()
        {
            try
            {
                //obtenemos una lista de empleados (vista personalizada)
                List<object> resultGetListTurnos = dbcmd.cmdTypeReader(33, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(resultGetListTurnos);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //casteamos a los modelos de datos reales
                List<list_turnos> list_turnos = resultGetListTurnos.OfType<list_turnos>().ToList();

                return View(list_turnos);
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UpdateTurno(int empleado_id)
        {
            try
            {
                return View(new turno() { empleado_id = empleado_id });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UpdateTurno(turno turno)
        {
            try
            {
                if (turno.tiempo_inicio > turno.tiempo_fin)
                {
                    ModelState.AddModelError("HoraInicio", "La hora de inicio esta despues de la hora de fin.");
                    return View(turno);
                }

                //se actualizan las horas
                theData.Add("turno", turno);
                object[] resultUpdateTurno = dbcmd.cmdTypeNonQuery(22, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultUpdateTurno);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });


                //obtenemos la persona asociada al empleado
                theData.Add("empleado", new empleado() { empleado_id = turno.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("persona", (persona)getPersonaResult[0]);

                //informamos al empleado
                object[] resultSendMail = mailcmd.SendEmail(4, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "Se ha actualizado el turno del empleado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddLibre(int empleado_id)
        {
            try
            {
                return View(new libre() { empleado_id = empleado_id });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddLibre(libre libre)
        {
            try
            {
                //se inserta la libre
                theData.Add("libre", libre);
                object[] resultInsertLibre = dbcmd.cmdTypeNonQuery(10, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertLibre);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la persona asociada al empleado
                theData.Add("empleado", new empleado() { empleado_id = libre.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("persona", (persona)getPersonaResult[0]);

                //informamos al empleado
                object[] resultSendMail = mailcmd.SendEmail(6, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "Se ha asignado la libre al empleado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult AddExtra(int empleado_id)
        {
            try
            {

                return View(new extra() { empleado_id = empleado_id });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddExtra(extra extra)
        {
            try
            {
                //se inserta la extra
                theData.Add("extra", extra);
                object[] resultInsertExtra = dbcmd.cmdTypeNonQuery(8, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultInsertExtra);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                //obtenemos la persona asociada al empleado
                theData.Add("empleado", new empleado() { empleado_id = extra.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, theData);
                TempData["failure"] = exmng.exceptionCheckerForList(getPersonaResult);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                theData.Add("persona", (persona)getPersonaResult[0]);

                //informamos al empleado
                object[] resultSendMail = mailcmd.SendEmail(5, theData);
                TempData["failure"] = exmng.exceptionCheckerForArray(resultSendMail);
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                TempData["success"] = "Se ha asignado la extra al empleado satisfactoriamente.";
                return RedirectToAction("Index", "Home", new { });
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            }
        }


        public Dictionary<string, object> ProcessRolAuxiliar(List<rol_empleado> og, rol_auxiliar aux, int id_empleado)
        {

            //cuando se obtienen desde la base de datos
            if (og != null)
            {

                aux = new rol_auxiliar();

                //iteramos a traves de todos los items de la lista
                og.ForEach(delegate (rol_empleado item)
                {
                    switch (item.rol_id)
                    {
                        case 1:
                            aux.isProprietario = true;
                            break;
                        case 2:
                            aux.isAdministrador = true;
                            break;
                        case 3:
                            aux.isContribuidor = true;
                            break;
                        case 4:
                            aux.isLector = true;
                            break;
                        case 5:
                            aux.isInvitado = true;
                            break;
                    }
                });
                return new Dictionary<string, object>() { { "rol_auxiliar", aux } };
            }

            //cuando se obtienen desde la vista
            if (aux != null)
            {
                og = new List<rol_empleado>();
                int i = 0;
                //iteramos a traves de todos los atributos del objeto
                foreach (var prop in aux.GetType().GetProperties())
                {
                    i++;
                    if ((bool)prop.GetValue(aux) == true)
                    {
                        og.Add(new rol_empleado { rol_id = i, empleado_id = id_empleado });
                    }
                }
                return new Dictionary<string, object>() { { "rol_empleado", og } };
            }

            return null;
        }

        public Dictionary<string, object> ProcessPermisoAuxiliar(List<permiso_empleado> og, permiso_auxiliar aux, int id_empleado)
        {

            //cuando se obtienen desde la base de datos
            if (og != null)
            {

                aux = new permiso_auxiliar();

                //iteramos a traves de todos los items de la lista
                og.ForEach(delegate (permiso_empleado item)
                {
                    switch (item.permiso_id)
                    {
                        case 1:
                            aux.hasAccesoTotal = true;
                            break;
                        case 2:
                            aux.hasEjecucion = true;
                            break;
                        case 3:
                            aux.hasEscritura = true;
                            break;
                        case 4:
                            aux.hasLectura = true;
                            break;
                        case 5:
                            aux.hasNinguno = true;
                            break;
                    }
                });
                return new Dictionary<string, object>() { { "permiso_auxiliar", aux } };
            }

            //cuando se obtienen desde la vista
            if (aux != null)
            {
                og = new List<permiso_empleado>();
                int i = 0;
                //iteramos a traves de todos los atributos del objeto
                foreach (var prop in aux.GetType().GetProperties())
                {
                    i++;
                    if ((bool)prop.GetValue(aux) == true)
                    {
                        og.Add(new permiso_empleado { permiso_id = i, empleado_id = id_empleado });
                    }
                }
                return new Dictionary<string, object>() { { "permiso_empleado", og } };
            }

            return null;
        }

    }
}

/*
 
             try
            {
                if (TempData["failure"] != null)
                    return RedirectToAction("Index", "Home", new { });

                return View();
            }
            catch (Exception ex)
            {
                TempData["failure"] = exmng.exceptionWriter(ex);
                return RedirectToAction("Index", "Home", new { });
            } 
 
 */

/*
             asi se hace una redireccion compleja con una accion y parametros
            return RedirectToAction("Index2", "Home", new { var1 = 0, var2 = 1, var3 = 2});
 */