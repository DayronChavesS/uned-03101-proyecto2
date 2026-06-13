using chaves_dayron_proyecto2_3101.Misc;
using System;
using System.Data.SqlClient;

namespace chaves_dayron_proyecto2_3101.DataBase
{
    public class DBConnection
    {
        ExceptionManager exmng = new ExceptionManager();

        protected SqlConnection connection;
        
        protected string strngCnnctnLocal = "Data Source=(local);Initial Catalog=chaves-dayron-proyecto2-3101_db;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        //debe ajustar este string al crear el servidor de base de datos en Azure
        protected string strngCnnctnAzure = "Data Source=tcp:chaves-dayron-proyecto2-3101dbserver.database.windows.net,1433;Initial Catalog=chaves-dayron-proyecto2-3101_db;User Id=chaves-dayron@chaves-dayron-proyecto2-3101dbserver;***REMOVED***";

        protected object[] ConnectToDB()
        {
            try
            {
                if (IsRunningInAzure())
                {
                    connection = new SqlConnection(strngCnnctnAzure);
                }
                else
                {
                    connection = new SqlConnection(strngCnnctnLocal);
                }

                //abrimos la conexion
                connection.Open();
                return new object[] { 0 };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        protected object[] DisconnectFromDB()
        {
            try
            {
                //cerramos la conexion
                connection.Close();
                return new object[] { 0 };
            }
            catch (Exception ex)
            {
                return new object[] { -1, exmng.exceptionWriter(ex) };
            }
        }

        public static bool IsRunningInAzure() { return System.Environment.GetEnvironmentVariable("RoleInstanceId") != null; }
    }
}