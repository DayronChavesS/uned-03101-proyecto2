using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace chaves_dayron_proyecto2_3101.Misc
{
    public class ExceptionManager
    {
        public string exceptionWriter(Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = new StringBuilder();

            traceString.AppendLine("Error: " + x.Message);

            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString.AppendLine("Archivo: " + frame.GetFileName());
                traceString.AppendLine("Metodo: " + frame.GetMethod().Name);
                traceString.AppendLine("Numero de Linea: " + frame.GetFileLineNumber());
                traceString.AppendLine("------");
            }

            return traceString.ToString();
        }

        public string exceptionCheckerForList(List<object> genericList)
        {
            try
            {
                //verificamos si se retorno error
                var check = genericList[0].GetType().GetProperty("result").GetValue(genericList[0], null);

                //si el casteo falla es porque no hubo excepcion
                if ((int)check == -1)
                {
                    return (string)genericList[0].GetType().GetProperty("message").GetValue(genericList[0], null);
                }

                return null;
            }
            catch (NullReferenceException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string exceptionCheckerForArray(object[] genericArray)
        {
            try
            {
                //verificamos si se retorno error
                //si el casteo falla es porque no hubo excepcion
                if ((int)genericArray[0] == -1)
                {
                    return (string)genericArray[1];
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}