using chaves_dayron_proyecto2_3101.DataBase;
using chaves_dayron_proyecto2_3101.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace chaves_dayron_proyecto2_3101.Misc
{
    public class Remind
    {
        /*
            La bandeja es simulada, y debe ingresar aqui para verla.
            Utilice las credenciales del documento escrito de este proyecto.
            https://mailtrap.io/signin
         */

        //credenciales de conexion a mailtrap
        protected string mailtrapUser = "171ce314fc8b1a";
        protected string mailtrapPassword = "24ce989e221e04";
        private Timer _timer = null;
        private DateTime now = DateTime.Now;
        ExceptionManager exmng = new ExceptionManager();
        DBCommand dbcmd = new DBCommand();

        //iniciar trabajo programado con tan solo crear el objeto
        //la creacion de este objeto se encuentra en Global.asax, que es el "main" de este proyecto y lo aclopa al backend.
        public Remind()
        {
            //Que se ejecute a las 00:00AM, cada 24 horas.
            StartTimer(new TimeSpan(0, 0, 1), new TimeSpan(24, 0, 0));

        }
        protected void StartTimer(TimeSpan scheduledRunTime, TimeSpan timeBetweenEachRun)
        {
            // obtener el tiempo actual
            double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
            // obtener el tiempo de proxima ejecucion
            double scheduledTime = scheduledRunTime.TotalMilliseconds;
            // obtener el tiempo entre intervalos
            double intervalPeriod = timeBetweenEachRun.TotalMilliseconds;

            //se determina cuanto tiempo falta para hacer la primera ejecucion (hoy, o mañana)
            double firstExecution = current > scheduledTime ? intervalPeriod - (current - scheduledTime) : scheduledTime - current;

            //Objeto que ejecutara nuestro codigo programado en horario
            TimerCallback callback = new TimerCallback(RunChecks);

            //Cronometro que ejecutara el codigo cada vez que el intervalo llegue a cero.
            _timer = new Timer(callback, null, Convert.ToInt32(firstExecution), Convert.ToInt32(intervalPeriod));

        }
        public void RunChecks(object state)
        {
            //la base de datos hace el trabajo de encontrar
            //aquellos elementos que ocurren al dia siguiente
            checkReuniones();
            checkLibres();
            checkExtras();
        }
        public void checkReuniones()
        {
            List<object> getReunionesResult = dbcmd.cmdTypeReader(34, new Dictionary<string, object>());

            if (getReunionesResult.Count == 0)
            {
                return;
            }

            List<reunion> listReuniones = getReunionesResult.OfType<reunion>().ToList();
            listReuniones.ForEach(delegate (reunion item)
            {
                Dictionary<string, object> emailData = new Dictionary<string, object>();

                emailData.Add("reunion", item);

                emailData.Add("empleado", new empleado() { empleado_id = item.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, emailData);

                emailData.Add("persona", (persona)getPersonaResult[0]);

                SendEmail(1, emailData);
            });
        }
        public void checkLibres()
        {
            List<object> getLibresResult = dbcmd.cmdTypeReader(35, new Dictionary<string, object>());

            if (getLibresResult.Count == 0)
            {
                return;
            }

            List<libre> listLibre = getLibresResult.OfType<libre>().ToList();
            listLibre.ForEach(delegate (libre item)
            {
                Dictionary<string, object> emailData = new Dictionary<string, object>();

                emailData.Add("libre", item);

                emailData.Add("empleado", new empleado() { empleado_id = item.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, emailData);

                emailData.Add("persona", (persona)getPersonaResult[0]);

                SendEmail(2, emailData);
            });
        }

        public void checkExtras()
        {
            List<object> getExtrasResult = dbcmd.cmdTypeReader(36, new Dictionary<string, object>());

            if (getExtrasResult.Count == 0)
            {
                return;
            }

            List<extra> listExtra = getExtrasResult.OfType<extra>().ToList();
            listExtra.ForEach(delegate (extra item)
            {
                Dictionary<string, object> emailData = new Dictionary<string, object>();

                emailData.Add("extra", item);

                emailData.Add("empleado", new empleado() { empleado_id = item.empleado_id });

                List<object> getPersonaResult = dbcmd.cmdTypeReader(13, emailData);

                emailData.Add("persona", (persona)getPersonaResult[0]);

                SendEmail(3, emailData);
            });
        }


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
                        contentResult = bodyRemindReunion(emailData);
                        break;
                    case 2:
                        contentResult = bodyRemindLibre(emailData);
                        break;
                    case 3:
                        contentResult = bodyRemindExtra(emailData);
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


        public object[] bodyRemindReunion(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                reunion reunion = (reunion)emailData["reunion"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Recordario de reunion.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le recordamos que para el dia de mañana tiene una reunion con RR.HH.</p>" +
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

        public object[] bodyRemindLibre(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                libre libre = (libre)emailData["libre"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Recordario de libre.";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le recordamos que no debe presentarse a trabajar el dia de mañana.</p>" +
                    "<p> Su libre fue aprobada: </p>" +
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

        public object[] bodyRemindExtra(Dictionary<string, object> emailData)
        {
            try
            {
                MimeMessage emailContent = new MimeMessage();
                persona persona = (persona)emailData["persona"];
                extra extra = (extra)emailData["extra"];

                emailContent.From.Add(new MailboxAddress("Tu Empresa", "tuempresa@example.com"));
                emailContent.To.Add(new MailboxAddress(persona.nombre, persona.correo));

                emailContent.Subject = "Recordario de extra";
                emailContent.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = "<h1>Hola " + persona.nombre + " " + persona.apellido + ".</h1><br/><br/>" +
                    "<p> Le recordamos que debe presentarse a trabajar el dia de mañana.</p>" +
                    "<p> Su extra fue aprobada: </p>" +
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
    }
}