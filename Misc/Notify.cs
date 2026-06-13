using chaves_dayron_proyecto2_3101.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;

namespace chaves_dayron_proyecto2_3101.Misc
{
    public class Notify
    {
        /*
            La bandeja es simulada, y debe ingresar aqui para verla.
            Utilice las credenciales del documento escrito de este proyecto.
            https://mailtrap.io/signin
         */

        //credenciales de conexion a mailtrap
        protected string mailtrapUser = "171ce314fc8b1a";
        protected string mailtrapPassword = "24ce989e221e04";
        ExceptionManager exmng = new ExceptionManager();
        public object[] SendEmail(int tipoCorreo, Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent;
                object[] contentResult;

                //creamos el contenido del correo electronico
                switch (tipoCorreo)
                {
                    case 1:
                        contentResult = registroEmpresa(emailData);
                        break;
                    case 2:
                        contentResult = cambioInformacionLaboral(emailData);
                        break;
                    case 3:
                        contentResult = cambioRolesPermisos(emailData);
                        break;
                    case 4:
                        contentResult = cambioDeTurno(emailData);
                        break;
                    case 5:
                        contentResult = asignacionDeExtra(emailData);
                        break;
                    case 6:
                        contentResult = asignacionDeLibre(emailData);
                        break;
                    case 7:
                        contentResult = programacionReunion(emailData);
                        break;
                    case 8:
                        contentResult = evaluacionRealizada(emailData);
                        break;
                    default:
                        return new object[] { -1, "Invalid Email Type" };
                }

                if ((int)contentResult[0] != 0)
                {
                    return contentResult;
                }

                emailContent = (MimeMessage)contentResult[1];

                //objeto que nos permite conectarnos a servidores SMTP
                SmtpClient smtp = new SmtpClient();

                /*
                 Nos conectamos y autentificacmos en Mailtrap segun documentacion.
                 https://help.mailtrap.io/article/109-getting-started-with-mailtrap-email-testing
                 */

                smtp.Connect("sandbox.smtp.mailtrap.io", 2525, false);
                smtp.Authenticate(mailtrapUser, mailtrapPassword);

                //enviamos el correo
                smtp.Send(emailContent);
                smtp.Disconnect(true);

                return new object[] { 0 };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] registroEmpresa(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                departamento departamento = (departamento)emailData["departamento"];
                puesto puesto = (puesto)emailData["puesto"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Bienvenido a Tu Empresa.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que a sido contratado en Tu Empresa. </p>" +
                    "<p> Su puesto es el siguiente: </p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Departamento: " + departamento.descripcion + "</b></p>" +
                    "<p><b>Puesto: " + puesto.descripcion + "</b></p>" +
                    "<p><b>Salario base: " + puesto.salario + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };
                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] cambioInformacionLaboral(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                departamento departamento = (departamento)emailData["departamento"];
                puesto puesto = (puesto)emailData["puesto"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Informacion laboral actualizada.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que han habido cambios en su puesto.</p>" +
                    "<p> Su nuevo puesto es el siguiente: </p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Departamento: " + departamento.descripcion + "</b></p>" +
                    "<p><b>Puesto: " + puesto.descripcion + "</b></p>" +
                    "<p><b>Salario base: " + puesto.salario + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] cambioRolesPermisos(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                rol rol = (rol)emailData["rol"];
                string emailBodyForPermiso = "";

                //pueden haber varios permisos por empleado
                //el cuerpo del correo para ese dato debe ser creado con antelacion aqui
                foreach (string key in emailData.Keys)
                {
                    if (key.Contains("permiso"))
                    {
                        permiso permiso = (permiso)emailData[key];
                        emailBodyForPermiso += "<p><b>" + permiso.descripcion + "</b></p></br>";
                    }
                }

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Cambio de roles y permisos.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que se han actualizado sus roles y permisos.</p>" +
                    "<p> Su rol actual es: " + rol.descripcion + "</p>" +
                    "<p> Y tiene los siguientes permisos: </p>" +
                    "<br/><hr/><br/>" +
                    emailBodyForPermiso +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] cambioDeTurno(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                turno turno = (turno)emailData["turno"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Nuevo turno asignado.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que se le ha asignado un nuevo turno: </p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Desde: " + turno.tiempo_inicio + "</b></p>" +
                    "<p><b>Hasta: " + turno.tiempo_fin + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] asignacionDeExtra(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                extra extra = (extra)emailData["extra"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Nueva extra asignada.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que su extra a sido aprobada.</p>" +
                    "<p> Esta extra tendra una bonificacion a su salario base para el proximo deposito.</p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Dia: " + extra.fecha + "</b></p>" +
                    "<p><b>Motivo: " + extra.motivo + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] asignacionDeLibre(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                libre libre = (libre)emailData["libre"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Nueva libre asignada.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que su libre a sido aprobada.</p>" +
                    "<p> Esta libre tendra una deduccion a su salario base para el proximo deposito.</p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Dia: " + libre.fecha + "</b></p>" +
                    "<p><b>Motivo: " + libre.motivo + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] programacionReunion(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                reunion reunion = (reunion)emailData["reunion"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Tiene una reunion programada.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que RR.HH a programado una reunion presencial con usted, " +
                    "con el objetivo de revisar y evaluar su desempeño en el ultimo mes.</p>" +
                    "<p> Horario de la reunion: </p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Fecha: " + reunion.fecha.ToString("dd-MM-yyyy") + "</b></p>" +
                    "<p><b>Inicio: " + reunion.hora_inicio + "</b></p>" +
                    "<p><b>Fin: " + reunion.hora_fin + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public object[] evaluacionRealizada(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                evaluacion evaluacion = (evaluacion)emailData["evaluacion"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Resultados de la evaluacion.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le informamos que los resultados de su ultima evaluacion con RR.HH ya estan disponibles.</p>" +
                    "<p> Resultados: </p>" +
                    "<br/><hr/><br/>" +
                    "<p><b>Objetivo: " + evaluacion.objetivo + "</b></p>" +
                    "<p><b>Retroalimentacion: " + evaluacion.retroalimentacion + "</b></p>" +
                    "<p><b>Seguimiento: " + evaluacion.seguimiento + "</b></p>" +
                    "<br/><hr/><br/>" +
                    "<p><i>Gracias por confiar en Tu Empresa.<i></p>" +
                    "<p><small><i>Este mensaje se genero el: " + DateTime.Now + ".<i></p></small>"
                };

                return new object[] { 0, emailContent };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }
    }
}