using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;

namespace DataImport
{
    public static class Utils
    {

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }
        }

        public static DateTime parseDate(string fecha, string fieldName, string expediente)
        {

            DateTime parsedDate = new DateTime(1900, 1, 1);

            try
            {
                parsedDate = (string.IsNullOrEmpty(fecha.Trim()) ? new DateTime(1900, 1, 1) : new DateTime(int.Parse(fecha.Substring(0, 4)), int.Parse(fecha.Substring(4, 2)), int.Parse(fecha.Substring(6, 2))));
            }
            catch
            {
                log.Error(string.Format("Error parsing date:[{0}] value:[{1}] expediente:[{2}]", fieldName, fecha, expediente));
            }

            return parsedDate;
        }

        public static DateTime DateCheck(DateTime fecha, string fieldName, string expediente)
        {

            DateTime parsedDate = new DateTime(1900, 1, 1);

            try
            {
                parsedDate = fecha;
            }
            catch
            {
                log.Error(string.Format("Error parsing date:[{0}] value:[{1}] expediente:[{2}]", fieldName, fecha, expediente));
            }

            return parsedDate;
        }


        public static TimeSpan parseTime(string no_time, string fieldName, string expediente)
        {
            TimeSpan time = new TimeSpan(0, 0, 0);

            try
            {
                var timex = no_time.Split(':').ToList<string>();
                if (timex.Count() == 2)
                    timex.Add("");

                if ((timex.Count() < 2))
                {
                    time = new TimeSpan(0, 0, 0);
                }
                else {
                    if (parseToInt(timex[0], "", "")<=23 && parseToInt(timex[1], "", "") <=59 && parseToInt(timex[2], "", "")<=59)
                        time = new TimeSpan(parseToInt(timex[0], fieldName, expediente), parseToInt(timex[1], fieldName, expediente), parseToInt(timex[2], fieldName, expediente));
                    else
                        log.Error(string.Format("Error parsing time:[{0}] value:[{1}] expediente:[{2}]", fieldName, no_time, expediente));
                }
            }
            catch
            {
                log.Error(string.Format("Error parsing time:[{0}] value:[{1}] expediente:[{2}]", fieldName, no_time, expediente));
            }

            return time;
        }

        public static int parseToInt(string value, string fieldName, string expediente)
        {
            var result = 0;
            try
            {
                if (!string.IsNullOrEmpty(value.Trim()))
                    result = int.Parse(value);
            }
            catch
            {
                log.Error(string.Format("Error parsing field:[{0}] value:[{1}] expediente:[{2}]", fieldName, value, expediente));
            }
            return result;
        }

        public static void updateMigracionLog(SqlConnection cnn, string tabla, int recordCounter)
        {
            try
            {
                int lastRecord = cnn.Query<int>("SELECT LastRecord FROM Migracion WHERE Tabla = @Tabla", new { Tabla = tabla }).FirstOrDefault();
                if (lastRecord == 0 && recordCounter > 0)
                { //if not records found, insert
                    cnn.Execute(@"INSERT INTO dbo.Migracion([Tabla], [LastRecord],[LastUpdated])
                                    VALUES(@Tabla, @LastRecord, @LastUpdated)",
                            new
                            {
                                Tabla = tabla,
                                LastRecord = recordCounter,
                                LastUpdated = new DateTime()
                            }
                        );
                }
                else if (recordCounter > 0)
                {
                    cnn.Execute(@"UPDATE Migracion SET LastRecord = @LastRecord, LastUpdated = @LastUpdated WHERE Tabla = @Tabla",
                            new
                            {
                                Tabla = tabla,
                                LastRecord = recordCounter,
                                LastUpdated = new DateTime()
                            }
                        );
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error updating Migracion Log {0} {1}:", tabla, recordCounter));
                log.Error(e.Message);
            }
        }

        public static int getLastRecordMigracionLog(SqlConnection cnn, string tabla)
        {
            int lastRecord = 0;
            try
            {
                lastRecord = cnn.Query<int>("SELECT LastRecord FROM Migracion WHERE Tabla = @Tabla", new { Tabla = tabla }).FirstOrDefault();                
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error getting Migracion Log {0}", tabla));
                log.Error(e.Message);
            }
            return lastRecord;
        }

        public static string getMigracionParam(SqlConnection cnn, string tabla)
        {
            string param = string.Empty;
            try
            {
                param = cnn.Query<string>("SELECT Parameter FROM Migracion WHERE Tabla = @Tabla", new { Tabla = tabla }).FirstOrDefault();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error getting Migracion Log {0}", tabla));
                log.Error(e.Message);
            }
            return param;
        }

        public static int getExpedienteIdDeMarcas(SqlConnection cnn, string nro_solic) {
            int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero AND ModuloId=1", 
                new { numero = nro_solic }).FirstOrDefault();
            return expedienteId;
        }

        public static int getExpedienteIdDePatentes(SqlConnection cnn, string idsolicitud, int tipoDeRegistroId)
        {
            int expedienteId = cnn.Query<int>(
                @"SELECT Id 
                FROM [dbo].[Expedientes] 
                WHERE Numero = @numero AND TipoDeRegistroId = @TipoDeRegistroId AND ModuloId=2",
                new { numero = idsolicitud, TipoDeRegistroId = tipoDeRegistroId }).FirstOrDefault();
            return expedienteId;
        }

        public static int getExpedienteIdDeDA(SqlConnection cnn, string idsolicitud)
        {
            int expedienteId = cnn.Query<int>(
                @"SELECT Id 
                FROM [dbo].[Expedientes] 
                WHERE Numero = @numero AND ModuloId=3",
                new { numero = idsolicitud }).FirstOrDefault();
            return expedienteId;
        }


        public static int getExpedienteIdDeAnotaciones(SqlConnection cnn, int tipoDeRegistroId, string nro_solic)
        {
            int expedienteId = cnn.Query<int>(
                @"SELECT Id 
                    FROM [dbo].[Expedientes] 
                    WHERE Numero = @numero AND TipoDeRegistroId=@TipoDeRegistroId AND ModuloId=4",
                new { numero = nro_solic, TipoDeRegistroId = tipoDeRegistroId }).FirstOrDefault();
            return expedienteId;
        }

        public static int getExpedienteIdDeRenovaciones(SqlConnection cnn, int tipoDeRegistroId, string nro_solic)
        {
            int expedienteId = cnn.Query<int>("SELECT Id FROM [dbo].[Expedientes] WHERE Numero = @numero AND TipoDeRegistroId=@TipoDeRegistroId AND ModuloId=5",
                new { numero = nro_solic, TipoDeRegistroId = tipoDeRegistroId }).FirstOrDefault();
            return expedienteId;
        }

        public static DateTime getInitDateForCrono() {
            string daysBackConfig = ConfigurationManager.AppSettings["crono.daysBack"];
            if (string.IsNullOrEmpty(daysBackConfig))
                daysBackConfig = "5";

            int daysBack = parseToInt(daysBackConfig, "", "") * -1;

            DateTime todayMinus5 = DateTime.Now.AddDays(daysBack);
            return todayMinus5;
        }

    }
}
