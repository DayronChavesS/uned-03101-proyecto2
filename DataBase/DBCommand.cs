using chaves_dayron_proyecto2_3101.Misc;
using chaves_dayron_proyecto2_3101.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace chaves_dayron_proyecto2_3101.DataBase
{
    public class DBCommand : DBConnection
    {
        /*
         Estructura general del codigo:
         -> Nos conectamos a la base de datos
         -> Inovocamos un procedimiento almacenado en SQL Server
         -> Si hay parametros (en procedimiento almacenado): 
            los agregamos uno por uno con el objeto enviado por parametro (de la funcion).
         -> Ejectuamos el procedimiento.
         -> Si el procedimiento es una consulta y retorna resultados:
            - Entramos en su ciclo hasta que el lector llegue a EOL.
            - Capturamos los datos en un objeto. Si son varias filas, creamos una lista de objetos.
         -> Nos desconectamos de la base da datos.
         */

        /*
         Aqui hay GETs, INSERTs y UPDATEs para cada una de las tablas de la base de datos.
         */

        /*
            No es facil acceder a la consola e informacion de depuracion en Web Apps
            Estos metodos estan creados para devolver las excepciones al controller
            Y poder imprimirlos en las vistas.

            Codigos de retorno:
            0: True || Sin Errores
            1: False
            -1: Error
        */

        ExceptionManager exmng = new ExceptionManager();

        public object[] cmdTypeHasRows(int theStp, Dictionary<string, object> theData)
        {
            try
            {
                object[] result = ConnectToDB();
                if ((int)result[0] != 0) { return result; }

                SqlCommand cmd;
                persona persona;

                switch (theStp)
                {
                    case 1:
                        cmd = new SqlCommand("stpVerifyCedula", connection);
                        persona = (persona)theData["persona"];
                        cmd.Parameters.AddWithValue("@cedula", persona.cedula);
                        break;
                    case 2:
                        cmd = new SqlCommand("stpVerifyCorreo", connection);
                        persona = (persona)theData["persona"];
                        cmd.Parameters.AddWithValue("@correo", persona.correo);
                        break;
                    default:
                        return new object[] { -1, "Invalid StoredProcedure" };
                }

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                int i = reader.HasRows ? 0 : 1;

                result = DisconnectFromDB();
                if ((int)result[0] != 0) { return result; }

                return new object[] { i };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] cmdTypeNonQuery(int theStp, Dictionary<string, object> theData)
        {
            try
            {
                object[] result = ConnectToDB();
                if ((int)result[0] != 0) { return result; }


                SqlCommand cmd;
                empleado empleado;
                evaluacion evaluacion;
                extra extra;
                historial historial;
                libre libre;
                permiso_empleado permisoEmpleado;
                persona persona;
                reunion reunion;
                rol_empleado rolEmpleado;
                salario salario;
                turno turno;
                porcentaje porcentaje;
                puesto puesto;

                switch (theStp)
                {
                    case 1:
                        cmd = new SqlCommand("stpDeleteExtra", connection);
                        extra = (extra)theData["extra"];
                        cmd.Parameters.Add("@extra_id", SqlDbType.Int).Value = extra.extra_id;
                        break;
                    case 2:
                        cmd = new SqlCommand("stpDeleteLibre", connection);
                        libre = (libre)theData["libre"];
                        cmd.Parameters.Add("@libre_id", SqlDbType.Int).Value = libre.libre_id;
                        break;
                    case 3:
                        cmd = new SqlCommand("stpDeletePermisoEmpleado", connection);
                        permisoEmpleado = (permiso_empleado)theData["permiso_empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = permisoEmpleado.empleado_id;
                        break;
                    case 4:
                        cmd = new SqlCommand("stpDeleteRolEmpleado", connection);
                        rolEmpleado = (rol_empleado)theData["rol_empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = rolEmpleado.empleado_id;
                        break;
                    case 5:
                        cmd = new SqlCommand("stpDeleteTurno", connection);
                        turno = (turno)theData["turno"];
                        cmd.Parameters.Add("@turno_id", SqlDbType.Int).Value = turno.turno_id;
                        break;
                    case 6:
                        cmd = new SqlCommand("stpInsertEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@persona_id", SqlDbType.Int).Value = empleado.persona_id;
                        cmd.Parameters.Add("@departamento_id", SqlDbType.Int).Value = empleado.departamento_id;
                        cmd.Parameters.Add("@puesto_id", SqlDbType.Int).Value = empleado.puesto_id;
                        cmd.Parameters.Add("@desde_fecha", SqlDbType.DateTime).Value = empleado.desde_fecha;
                        break;
                    case 7:
                        cmd = new SqlCommand("stpInsertEvaluacion", connection);
                        evaluacion = (evaluacion)theData["evaluacion"];
                        cmd.Parameters.Add("@reunion_id", SqlDbType.Int).Value = evaluacion.reunion_id;
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = evaluacion.empleado_id;
                        cmd.Parameters.Add("@objetivo", SqlDbType.VarChar).Value = evaluacion.objetivo;
                        cmd.Parameters.Add("@seguimiento", SqlDbType.VarChar).Value = evaluacion.seguimiento;
                        cmd.Parameters.Add("@retroalimentacion", SqlDbType.VarChar).Value = evaluacion.retroalimentacion;
                        break;
                    case 8:
                        cmd = new SqlCommand("stpInsertExtra", connection);
                        extra = (extra)theData["extra"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = extra.empleado_id;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = extra.fecha;
                        cmd.Parameters.Add("@motivo", SqlDbType.VarChar).Value = extra.motivo;
                        break;
                    case 9:
                        cmd = new SqlCommand("stpInsertHistorial", connection);
                        historial = (historial)theData["historial"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = historial.empleado_id;
                        break;
                    case 10:
                        cmd = new SqlCommand("stpInsertLibre", connection);
                        libre = (libre)theData["libre"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = libre.empleado_id;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = libre.fecha;
                        cmd.Parameters.Add("@motivo", SqlDbType.VarChar).Value = libre.motivo;
                        break;
                    case 11:
                        cmd = new SqlCommand("stpInsertPermisoEmpleado", connection);
                        permisoEmpleado = (permiso_empleado)theData["permiso_empleado"];
                        cmd.Parameters.Add("@permiso_id", SqlDbType.Int).Value = permisoEmpleado.permiso_id;
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = permisoEmpleado.empleado_id;
                        break;
                    case 12:
                        cmd = new SqlCommand("stpInsertPersona", connection);
                        persona = (persona)theData["persona"];
                        cmd.Parameters.Add("@cedula", SqlDbType.VarChar).Value = persona.cedula;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = persona.nombre;
                        cmd.Parameters.Add("@apellido", SqlDbType.VarChar).Value = persona.apellido;
                        cmd.Parameters.Add("@direccion", SqlDbType.VarChar).Value = persona.direccion;
                        cmd.Parameters.Add("@correo", SqlDbType.VarChar).Value = persona.correo;
                        cmd.Parameters.Add("@telefono", SqlDbType.VarChar).Value = persona.telefono;
                        break;
                    case 13:
                        cmd = new SqlCommand("stpInsertReunion", connection);
                        reunion = (reunion)theData["reunion"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = reunion.empleado_id;
                        cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = reunion.fecha;
                        cmd.Parameters.Add("@hora_inicio", SqlDbType.Time).Value = reunion.hora_inicio;
                        cmd.Parameters.Add("@hora_fin", SqlDbType.Time).Value = reunion.hora_fin;
                        break;
                    case 14:
                        cmd = new SqlCommand("stpInsertRolEmpleado", connection);
                        rolEmpleado = (rol_empleado)theData["rol_empleado"];
                        cmd.Parameters.Add("@rol_id", SqlDbType.Int).Value = rolEmpleado.rol_id;
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = rolEmpleado.empleado_id;
                        break;
                    case 15:
                        cmd = new SqlCommand("stpInsertSalario", connection);
                        salario = (salario)theData["salario"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = salario.empleado_id;
                        cmd.Parameters.Add("@desde_fecha", SqlDbType.Date).Value = salario.desde_fecha;
                        cmd.Parameters.Add("@hasta_fecha", SqlDbType.Date).Value = salario.hasta_fecha;
                        break;
                    case 16:
                        cmd = new SqlCommand("stpInsertTurno", connection);
                        turno = (turno)theData["turno"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = turno.empleado_id;
                        cmd.Parameters.Add("@tiempo_inicio", SqlDbType.Time).Value = turno.tiempo_inicio;
                        cmd.Parameters.Add("@tiempo_fin", SqlDbType.Time).Value = turno.tiempo_fin;
                        break;
                    case 17:
                        cmd = new SqlCommand("stpUpdateEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        cmd.Parameters.Add("@persona_id", SqlDbType.Int).Value = empleado.persona_id;
                        cmd.Parameters.Add("@departamento_id", SqlDbType.Int).Value = empleado.departamento_id;
                        cmd.Parameters.Add("@puesto_id", SqlDbType.Int).Value = empleado.puesto_id;
                        cmd.Parameters.Add("@desde_fecha", SqlDbType.DateTime).Value = empleado.desde_fecha;
                        break;
                    case 18:
                        cmd = new SqlCommand("stpUpdatePersona", connection);
                        persona = (persona)theData["persona"];
                        cmd.Parameters.Add("@persona_id", SqlDbType.VarChar).Value = persona.persona_id;
                        cmd.Parameters.Add("@cedula", SqlDbType.VarChar).Value = persona.cedula;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = persona.nombre;
                        cmd.Parameters.Add("@apellido", SqlDbType.VarChar).Value = persona.apellido;
                        cmd.Parameters.Add("@direccion", SqlDbType.VarChar).Value = persona.direccion;
                        cmd.Parameters.Add("@correo", SqlDbType.VarChar).Value = persona.correo;
                        cmd.Parameters.Add("@telefono", SqlDbType.VarChar).Value = persona.telefono;
                        break;
                    case 19:
                        cmd = new SqlCommand("stpUpdateSalarioEmpleado", connection);
                        salario = (salario)theData["salario"];
                        cmd.Parameters.Add("@salario_id", SqlDbType.Int).Value = salario.salario_id;
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = salario.empleado_id;
                        cmd.Parameters.Add("@desde_fecha", SqlDbType.Date).Value = salario.desde_fecha;
                        cmd.Parameters.Add("@hasta_fecha", SqlDbType.Date).Value = salario.hasta_fecha;
                        break;
                    case 20:
                        cmd = new SqlCommand("stpUpdateSalarioPuesto", connection);
                        puesto = (puesto)theData["puesto"];
                        cmd.Parameters.Add("@puesto_id", SqlDbType.Int).Value = puesto.puesto_id;
                        cmd.Parameters.Add("@salario", SqlDbType.Decimal).Value = puesto.salario;
                        break;
                    case 21:
                        cmd = new SqlCommand("stpUpdatePorcentaje", connection);
                        porcentaje = (porcentaje)theData["porcentaje"];
                        cmd.Parameters.Add("@libre_deduccion", SqlDbType.Decimal).Value = porcentaje.libre_deduccion;
                        cmd.Parameters.Add("@extra_bonificacion", SqlDbType.Decimal).Value = porcentaje.extra_bonificacion;
                        break;
                    case 22:
                        cmd = new SqlCommand("stpUpdateTurno", connection);
                        turno = (turno)theData["turno"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = turno.empleado_id;
                        cmd.Parameters.Add("@tiempo_inicio", SqlDbType.Time).Value = turno.tiempo_inicio;
                        cmd.Parameters.Add("@tiempo_fin", SqlDbType.Time).Value = turno.tiempo_fin;
                        break;
                    default:
                        return new object[] { -1, "Invalid StoredProcedure" };
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();

                result = DisconnectFromDB();
                if ((int)result[0] != 0) { return result; }


                return new object[] { 0 };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public List<object> cmdTypeReader(int theStp, Dictionary<string, object> theData)
        {
            List<object> dbRows = new List<object>();

            try
            {
                object[] result = ConnectToDB();
                if ((int)result[0] != 0) { return new List<object>(result); }

                SqlCommand cmd;
                empleado empleado;
                evaluacion evaluacion;
                extra extra;
                historial historial;
                libre libre;
                permiso permiso;
                permiso_empleado permisoEmpleado;
                persona persona;
                reunion reunion;
                rol rol;
                rol_empleado rolEmpleado;
                salario salario;
                turno turno;

                switch (theStp)
                {
                    case 1:
                        cmd = new SqlCommand("stpGetDepartamentos", connection);
                        break;
                    case 2:
                        cmd = new SqlCommand("stpGetEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 3:
                        cmd = new SqlCommand("stpGetEmpleados", connection);
                        break;
                    case 4:
                        cmd = new SqlCommand("stpGetEvaluacion", connection);
                        evaluacion = (evaluacion)theData["evaluacion"];
                        cmd.Parameters.Add("@evaluacion_id", SqlDbType.Int).Value = evaluacion.evaluacion_id;
                        break;
                    case 5:
                        cmd = new SqlCommand("stpGetExtraEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 6:
                        cmd = new SqlCommand("stpGetExtras", connection);
                        break;
                    case 7:
                        cmd = new SqlCommand("stpGetHistorial", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 8:
                        cmd = new SqlCommand("stpGetLibreEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 9:
                        cmd = new SqlCommand("stpGetLibres", connection);
                        break;
                    case 10:
                        cmd = new SqlCommand("stpGetNomina", connection);
                        break;
                    case 11:
                        cmd = new SqlCommand("stpGetPermisoEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 12:
                        cmd = new SqlCommand("stpGetPermisos", connection);
                        break;
                    case 13:
                        cmd = new SqlCommand("stpGetPersona", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 14:
                        cmd = new SqlCommand("stpGetPersonas", connection);
                        break;
                    case 15:
                        cmd = new SqlCommand("stpGetPuestos", connection);
                        break;
                    case 16:
                        cmd = new SqlCommand("stpGetReunionEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 17:
                        cmd = new SqlCommand("stpGetReuniones", connection);
                        break;
                    case 18:
                        cmd = new SqlCommand("stpGetRolEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 19:
                        cmd = new SqlCommand("stpGetRoles", connection);
                        break;
                    case 20:
                        cmd = new SqlCommand("stpGetSalarioEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 21:
                        cmd = new SqlCommand("stpGetSalarios", connection);
                        break;
                    case 22:
                        cmd = new SqlCommand("stpGetSumatoriaNomina", connection);
                        break;
                    case 23:
                        cmd = new SqlCommand("stpGetTurnoEmpleado", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@empleado_id", SqlDbType.Int).Value = empleado.empleado_id;
                        break;
                    case 24:
                        cmd = new SqlCommand("stpGetTurnos", connection);
                        break;
                    case 25:
                        cmd = new SqlCommand("stpGetPersonaWithCedula", connection);
                        persona = (persona)theData["persona"];
                        cmd.Parameters.Add("@cedula", SqlDbType.VarChar).Value = persona.cedula;
                        break;
                    case 26:
                        cmd = new SqlCommand("stpGetDepartamento", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@departamento_id", SqlDbType.Int).Value = empleado.departamento_id;
                        break;
                    case 27:
                        cmd = new SqlCommand("stpGetPuesto", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@puesto_id", SqlDbType.Int).Value = empleado.puesto_id;
                        break;
                    case 28:
                        cmd = new SqlCommand("stpGetPermiso", connection);
                        permiso = (permiso)theData["permiso"];
                        cmd.Parameters.Add("@permiso_id", SqlDbType.Int).Value = permiso.permiso_id;
                        break;
                    case 29:
                        cmd = new SqlCommand("stpGetRol", connection);
                        rol = (rol)theData["rol"];
                        cmd.Parameters.Add("@rol_id", SqlDbType.Int).Value = rol.rol_id;
                        break;
                    case 30:
                        cmd = new SqlCommand("stpGetListEmpleados", connection);
                        break;
                    case 31:
                        cmd = new SqlCommand("stpGetPorcentajes", connection);
                        break;
                    case 32:
                        cmd = new SqlCommand("stpGetEmpleadoWithPersonaId", connection);
                        empleado = (empleado)theData["empleado"];
                        cmd.Parameters.Add("@persona_id", SqlDbType.Int).Value = empleado.persona_id;
                        break;
                    case 33:
                        cmd = new SqlCommand("stpGetListTurnos", connection);
                        break;
                    case 34:
                        cmd = new SqlCommand("stpGetReunionesToRemind", connection);
                        break;
                    case 35:
                        cmd = new SqlCommand("stpGetLibresToRemind", connection);
                        break;
                    case 36:
                        cmd = new SqlCommand("stpGetExtrasToRemind", connection);
                        break;
                    default:
                        object o = new { result = -1, message = "Invalid StoredProcedure" };
                        dbRows.Add(o);
                        return dbRows;
                }

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                var parser = reader.GetRowParser<object>(typeof(object));

                switch (theStp)
                {
                    case 1:
                        parser = reader.GetRowParser<departamento>(typeof(departamento));
                        break;
                    case 2:
                        parser = reader.GetRowParser<empleado>(typeof(empleado));
                        break;
                    case 3:
                        parser = reader.GetRowParser<empleado>(typeof(empleado));
                        break;
                    case 4:
                        parser = reader.GetRowParser<evaluacion>(typeof(evaluacion));
                        break;
                    case 5:
                        parser = reader.GetRowParser<extra>(typeof(extra));
                        break;
                    case 6:
                        parser = reader.GetRowParser<extra>(typeof(extra));
                        break;
                    case 7:
                        parser = reader.GetRowParser<historial>(typeof(historial));
                        break;
                    case 8:
                        parser = reader.GetRowParser<libre>(typeof(libre));
                        break;
                    case 9:
                        parser = reader.GetRowParser<libre>(typeof(libre));
                        break;
                    case 10:
                        parser = reader.GetRowParser<nomina_detalle>(typeof(nomina_detalle));
                        break;
                    case 11:
                        parser = reader.GetRowParser<permiso_empleado>(typeof(permiso_empleado));
                        break;
                    case 12:
                        parser = reader.GetRowParser<permiso>(typeof(permiso));
                        break;
                    case 13:
                        parser = reader.GetRowParser<persona>(typeof(persona));
                        break;
                    case 14:
                        parser = reader.GetRowParser<persona>(typeof(persona));
                        break;
                    case 15:
                        parser = reader.GetRowParser<puesto>(typeof(puesto));
                        break;
                    case 16:
                        parser = reader.GetRowParser<reunion>(typeof(reunion));
                        break;
                    case 17:
                        parser = reader.GetRowParser<reunion>(typeof(reunion));
                        break;
                    case 18:
                        parser = reader.GetRowParser<rol_empleado>(typeof(rol_empleado));
                        break;
                    case 19:
                        parser = reader.GetRowParser<rol>(typeof(rol));
                        break;
                    case 20:
                        parser = reader.GetRowParser<salario>(typeof(salario));
                        break;
                    case 21:
                        parser = reader.GetRowParser<salario>(typeof(salario));
                        break;
                    case 22:
                        parser = reader.GetRowParser<nomina_sumatoria>(typeof(nomina_sumatoria));
                        break;
                    case 23:
                        parser = reader.GetRowParser<turno>(typeof(turno));
                        break;
                    case 24:
                        parser = reader.GetRowParser<turno>(typeof(turno));
                        break;
                    case 25:
                        parser = reader.GetRowParser<persona>(typeof(persona));
                        break;
                    case 26:
                        parser = reader.GetRowParser<departamento>(typeof(departamento));
                        break;
                    case 27:
                        parser = reader.GetRowParser<puesto>(typeof(puesto));
                        break;
                    case 28:
                        parser = reader.GetRowParser<permiso>(typeof(permiso));
                        break;
                    case 29:
                        parser = reader.GetRowParser<rol>(typeof(rol));
                        break;
                    case 30:
                        parser = reader.GetRowParser<list_empleados>(typeof(list_empleados));
                        break;
                    case 31:
                        parser = reader.GetRowParser<porcentaje>(typeof(porcentaje));
                        break;
                    case 32:
                        parser = reader.GetRowParser<empleado>(typeof(empleado));
                        break;
                    case 33:
                        parser = reader.GetRowParser<list_turnos>(typeof(list_turnos));
                        break;
                    case 34:
                        parser = reader.GetRowParser<reunion>(typeof(reunion));
                        break;
                    case 35:
                        parser = reader.GetRowParser<libre>(typeof(libre));
                        break;
                    case 36:
                        parser = reader.GetRowParser<extra>(typeof(extra));
                        break;
                    default:
                        object o = new { result = -1, message = "Invalid Parser" };
                        dbRows.Add(o);
                        return dbRows;
                }

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var obj = parser(reader);
                        dbRows.Add(obj);
                    }
                }

                result = DisconnectFromDB();
                if ((int)result[0] != 0) { return new List<object>(result); }


                return dbRows;
            }
            catch (Exception ex)
            {
                object o = new { result = -1, message = exmng.exceptionWriter(ex) };
                dbRows.Add(o);
                return dbRows;
            }
        }
    }
}