using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Configuration;

namespace DataImport
{
    public class Patentes
    {
        public static string ConnectionString;

        static string dbaseConnString = "Provider=vfpoledb;Data Source={0};Extended Properties=dBASE IV;";

        public static SqlConnection connection;

        public static SqlConnection GetOpenConnection()
        {
            ConnectionString = ConfigurationManager.AppSettings["rpiDb"];
            if (connection == null)
            {
                connection = new SqlConnection(ConnectionString);
                connection.Open();
            }
            return connection;
        }

        public static OleDbConnection GetdbaseOpenConnection()
        {
            string FilePath = ConfigurationManager.AppSettings["legacyPatentes"];

            var conn = new OleDbConnection(string.Format(dbaseConnString, FilePath));
            conn.Open();
            return conn;

        }

        private static SqlConnection cnn = GetOpenConnection();
        private static OleDbConnection dbaseConn = GetdbaseOpenConnection();

        static Patentes() {
            cnn = GetOpenConnection();
            dbaseConn = GetdbaseOpenConnection();
        }

        public static void ImportEstatus()
        {
            //string DBF_FileName = "status";
            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            //var dataReader = command.ExecuteReader();
            //while (dataReader.Read())
            //{              
            //    string idstatus = dataReader.GetValue(0).ToString();
            //    string descripcion = dataReader.GetString(2);

            //    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Estatus([Codigo],[Descripcion],[ModuloId])
            //        VALUES( @Codigo, @Descripcion, @ModuloId);
            //        SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
            //        new 
            //        { 
            //            Codigo=idstatus, 
            //            Descripcion=descripcion,  
            //            ModuloId = 2
            //        }
            //        ).Single();
            //}
            //dataReader.Close();
        }

        public static void ImportAgentes()
        {
            string DBF_FileName = "agentes";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            string idagente = string.Empty;
            string agente = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idagente = dataReader.GetValue(0).ToString();
                    agente = dataReader.GetString(1);
                    string domicilio = dataReader.GetString(2);
                    string telefono = dataReader.GetString(3);
                    string fax = dataReader.GetString(4);

                    cnn.Execute(@"MERGE INTO patentes.Agentes as t1 
                        using(SELECT @Nombre, @Domicilio, @Telefono, @Fax, @tmpId) AS t2 
                        (Nombre, Domicilio, Telefono, Fax, tmpId)
                        ON (t1.tmpId = t2.tmpId)
                        WHEN MATCHED THEN
                        UPDATE SET
                            t1.Nombre = t2.Nombre,
                            t1.Domicilio = t2.Domicilio,
                            t1.Telefono = t2.Telefono,
                            t1.Fax = t2.Fax
                        WHEN NOT MATCHED THEN
                            insert (Nombre, Domicilio, Telefono, Fax, tmpId)
                            values(t2.Nombre, t2.Domicilio, t2.Telefono, t2.Fax, t2.tmpId);",
                            new
                        {
                            Nombre = agente,
                            Domicilio = domicilio,
                            Telefono = telefono,
                            Fax = fax,
                            tmpId = idagente,
                        }
                    );
                
                    log.Error("agentes:" + agente);

                }
                catch (Exception e)
                {
                    log.Error(string.Format("Agente {0}, {1} tiene errores", idagente, agente));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        public static void ImportClasificaciones()
        {
            //string DBF_FileName = "clasificacion";
            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            //var dataReader = command.ExecuteReader();
            //while (dataReader.Read())
            //{              
            //    string idclasifica = dataReader.GetValue(0).ToString();
            //    string descripcion = dataReader.GetString(1);

            //    int expedienteId = cnn.Query<int>(@"INSERT INTO patentes.Clasificaciones([Descripcion],[tmpId])
            //        VALUES( @Descripcion, @tmpId);
            //        SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
            //        new 
            //        { 
            //            Descripcion=descripcion, 
            //            tmpId=idclasifica
            //        }
            //        ).Single();
            //}
            //dataReader.Close();
        }

        public static void ImportExpedientes()
        {
            string DBF_FileName = "mainpat";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromMainPat = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromMainPat, dbaseConn);
            DateTime timeNow = DateTime.Now;
            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {              
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    DateTime? fecha_solicitud = dataReader.GetDateTime(2); //date
                    string horaingreso = dataReader.GetString(3);
                    string registro = dataReader.GetValue(4).ToString();
                    string idstatus = dataReader.GetString(5);
                    DateTime? fecha_status = dataReader.GetDateTime(6);

                    string idagente = dataReader.GetValue(11).ToString();
                    string nanualidades = dataReader.GetValue(12).ToString();
                    string folio = dataReader.GetValue(13).ToString();
                    string tomo = dataReader.GetValue(14).ToString();
                    string descripcion = dataReader.GetString(15);
                    string ley = dataReader.GetString(16);

                    string resumen = dataReader.GetString(18);
                    string iduser_rec = dataReader.GetString(23);
                    DateTime? fecha_rec = dataReader.GetDateTime(24);
                    string idclasifica = dataReader.GetString(27);

                    string pct_id = dataReader.GetString(32);
                    DateTime? pct_fecha = dataReader.GetDateTime(33);
                    string citaciones = dataReader.GetString(34);

                    TimeSpan time = Utils.parseTime(horaingreso, DBF_FileName + "(horaingreso)", idsolicitud);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int found = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int agenteId = cnn.Query<int>("select Id FROM patentes.Agentes WHERE tmpId like @tmpId", new { tmpId = idagente }).FirstOrDefault();

                    int clasificacionId = cnn.Query<int>("select Id FROM patentes.Clasificaciones WHERE tmpId like @tmpId", new { tmpId = idclasifica }).FirstOrDefault();

                    int expedienteId = 0;

                    if (found == 0){
                        var xp = new
                        {
                            ModuloId = 2,
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = idsolicitud,
                            FechaDeSolicitud = fecha_solicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fecha_status,
                            LeyId = ley,
                            FechaActualizacion = timeNow
                        };

                        expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId],[FechaActualizacion])
                        VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId, @FechaActualizacion);
                        SELECT SCOPE_IDENTITY() AS [expedienteId]; ", xp).Single();
                    }
                    else {
                        var xp = new
                        {
                            ModuloId = 2,
                            TipoDeRegistroId = tipoDeRegistroId,
                            Numero = idsolicitud,
                            FechaDeSolicitud = fecha_solicitud,
                            Hora = time,
                            EstatusId = statusId,
                            FechaDeEstatus = fecha_status,
                            LeyId = ley,
                            FechaActualizacion = timeNow,
                            expedienteId = found
                        };

                        expedienteId = cnn.Query<int>(@"UPDATE dbo.Expedientes
                            SET FechaDeSolicitud = @FechaDeSolicitud,
                                Hora = @Hora,
                                EstatusId = @EstatusId,
                                FechaDeEstatus = @FechaDeEstatus,
                                FechaActualizacion = @FechaActualizacion
                            WHERE Id = @expedienteId;
                            SELECT @expedienteId AS [expedienteId]; ", xp).Single();
                    }

                    var pat = new
                    {
                        ExpedienteId = expedienteId,
                        Descripcion = descripcion,
                        Registro = registro,
                        AgenteId = agenteId,
                        anualidades = nanualidades,
                        Folio = folio,
                        Tomo = tomo,
                        Resumen = resumen,
                        ClasificacionId = clasificacionId,
                        RecibidoPorUsuarioId = 0,
                        FechaRecepcion = fecha_rec,
                        Pct = pct_id,
                        Fecha_Pct = pct_fecha,
                        Citaciones = citaciones
                    };

                    cnn.Execute(@"MERGE INTO patentes.Patentes as t1 
                        using(SELECT @ExpedienteId,
                            @Descripcion,
                            @Registro,
                            @AgenteId,
                            @anualidades,
                            @Folio,
                            @Tomo,
                            @Resumen,
                            @ClasificacionId,
                            @RecibidoPorUsuarioId,
                            @FechaRecepcion,
                            @Pct,
                            @Fecha_Pct,
                            @Citaciones) AS t2 
                                   (ExpedienteId,
                                    Descripcion,
                                    Registro,
                                    AgenteId,
                                    anualidades,
                                    Folio,
                                    Tomo,
                                    Resumen,
                                    ClasificacionId,
                                    RecibidoPorUsuarioId,
                                    FechaRecepcion,
                                    Pct,
                                    Fecha_Pct,
                                    Citaciones)
                        ON (t1.ExpedienteId = t2.ExpedienteId)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.Descripcion = t2.Descripcion,
                           t1.Registro = t2.Registro,
                           t1.AgenteId = t2.AgenteId,
                           t1.anualidades = t2.anualidades,
                           t1.Folio = t2.Folio,
                           t1.Tomo = t2.Tomo,
                           t1.Resumen = t2.Resumen,
                           t1.ClasificacionId = t2.ClasificacionId,
                           t1.Pct = t2.Pct,
                           t1.Fecha_Pct = t2.Fecha_Pct,
                           t1.Citaciones = t2.Citaciones
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, Descripcion, Registro, AgenteId, anualidades, Folio, Tomo,
                                    Resumen, ClasificacionId, RecibidoPorUsuarioId, FechaRecepcion, Pct, Fecha_Pct, Citaciones) 
                           values(t2.ExpedienteId, t2.Descripcion, t2.Registro, t2.AgenteId, t2.anualidades, t2.Folio, t2.Tomo,
                                    t2.Resumen, t2.ClasificacionId, t2.RecibidoPorUsuarioId, t2.FechaRecepcion, t2.Pct, t2.Fecha_Pct, t2.Citaciones);", pat );

                    log.console(string.Format("mainpat {0}:", expedienteId));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        public static void ImportCrono()
        {

            DateTime todayMinus5 = Utils.getInitDateForCrono();
            string DBF_FileName = "cronologia";

            string allCrono = Utils.getMigracionParam(cnn, "pat.crono");

            string selectFromCronologia = String.Empty;
            if (allCrono.Equals("1")) //all crono
            {
                selectFromCronologia = string.Format("select * from {0}", DBF_FileName);
            }
            else
            { //last few days
                 selectFromCronologia = string.Format("select * from {0} WHERE fecha >= DATE({1}, {2}, {3}) ORDER BY fecha",
                                        DBF_FileName,
                                        todayMinus5.Year.ToString(),
                                        todayMinus5.Month.ToString(),
                                        todayMinus5.Day.ToString());
            }

            OleDbCommand command = new OleDbCommand(selectFromCronologia, dbaseConn);

            var dataReader = command.ExecuteReader();
            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);

                    DateTime fecha = dataReader.GetDateTime(2);
                    string idstatus = dataReader.GetString(4);
                    string referencia = dataReader.GetString(5);
                    string idusuario = dataReader.GetString(6);
                    string observaciones = dataReader.GetString(7);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=2 AND Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int found = cnn.Query<int>("SELECT id FROM Cronologia WHERE ExpedienteId = @ExpedienteId AND Fecha = @Fecha AND EstatusId = @EstatusId",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Fecha = fecha,
                                EstatusId = statusId
                            }).FirstOrDefault();

                    if (expedienteId>0 && found == 0) //if evento cronologico not found
                    {
                        cnn.Execute(@"INSERT INTO dbo.Cronologia([ExpedienteId]
                            ,[Fecha]
                            ,[EstatusId]
                            ,[Referencia]
                            ,[UsuarioId]
                            ,[Observaciones]
                            ,[UsuarioIniciales])
                        VALUES(@ExpedienteId,
                            @Fecha,
                            @EstatusId,
                            @Referencia,
                            @UsuarioId,
                            @Observaciones,
                            @UsuarioIniciales
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 Fecha = fecha,
                                 EstatusId = statusId,
                                 Referencia = referencia,
                                 UsuarioId = 0,
                                 Observaciones = observaciones,
                                 UsuarioIniciales = idusuario
                             }
                            );
                    }

                    log.console(string.Format("Crono {0}:", idsolicitud));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        }

        /// ABANDONO
        public static void ImportAbandono()
        {
            string DBF_FileName = "abandono";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromAbandono = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAbandono, dbaseConn);

            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    string tipo_resolucion = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string choices = dataReader.GetString(4);
                    DateTime fecha1 = dataReader.GetDateTime(5);
                    DateTime fecha2 = dataReader.GetDateTime(6);
                    DateTime fecha3 = dataReader.GetDateTime(7);
                    DateTime fecha4 = dataReader.GetDateTime(8);
                    string observaciones = dataReader.GetString(9);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();

                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    if (expedienteId > 0)
                    {

                        cnn.Execute(@"MERGE INTO patentes.Abandonos as t1 
                        using(SELECT @ExpedienteId,
                                    @TipoResolucion,
                                    @Fecha,
                                    @Opciones,
                                    @Fecha1,
                                    @Fecha2,
                                    @Fecha3,
                                    @Fecha4,
                                    @Observaciones) AS t2 
                                   (ExpedienteId,TipoResolucion,Fecha,Opciones,Fecha1,Fecha2,Fecha3,Fecha4,Observaciones)
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TipoResolucion = t2.TipoResolucion)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.Fecha = t2.Fecha,
                           t1.Opciones = t2.Opciones,
                           t1.Fecha1 = t2.Fecha1,
                           t1.Fecha2 = t2.Fecha2,
                           t1.Fecha3 = t2.Fecha3,
                           t1.Fecha4 = t2.Fecha4
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId,TipoResolucion,Fecha,Opciones,Fecha1,Fecha2,Fecha3,Fecha4,Observaciones) 
                           values(t2.ExpedienteId, t2.TipoResolucion, t2.Fecha, t2.Opciones, t2.Fecha1, t2.Fecha2, t2.Fecha3, t2.Fecha4, t2.Observaciones);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                TipoResolucion = tipo_resolucion,
                                Fecha = fecha,
                                Opciones = choices,
                                Fecha1 = fecha1,
                                Fecha2 = fecha2,
                                Fecha3 = fecha3,
                                Fecha4 = fecha4,
                                Observaciones = observaciones
                            }
                        );
                    }
                    else {
                        log.Error("Abandonos: " + expedienteId);
                    }
                    log.console("Abandonos: " + expedienteId);

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Abandono {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END ABANDONO


        /// Anualidades
        public static void ImportAnualidades()
        {
            string DBF_FileName = "anualidades";
            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromAnualidades = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAnualidades, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string tipo_registro = string.Empty;

            while (dataReader.Read())
            {
                try
                {

                    idsolicitud = dataReader.GetString(0);
                    tipo_registro = dataReader.GetValue(1).ToString();

                    int numeroAnualidad = int.Parse(dataReader.GetValue(2).ToString());
                    DateTime fAnualidad = dataReader.GetDateTime(3);
                    string esrenovacion = dataReader.GetValue(4).ToString();
                    DateTime fVencimiento = dataReader.GetDateTime(5);
                    double valor = double.Parse(dataReader.GetValue(2).ToString());
                    string recibo = dataReader.GetString(7);
                    DateTime fRecibo = dataReader.GetDateTime(8);
                    string usuario = string.Empty; // PENDIENTE!!
                    string observaciones = dataReader.GetString(9);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.Anualidades as t1 
                        using(SELECT @ExpedienteId,
                                      @NumeroAnualidad,
                                      @EsRenovacion,
                                      @FechaVencimiento,
                                      @Valor,
                                      @Recibo,
                                      @FechaRecibo,
                                      @UsuarioId,
                                      @Observaciones,
                                      @FechaAnualidad) AS t2 
                                   (ExpedienteId,NumeroAnualidad,EsRenovacion,FechaVencimiento,Valor,Recibo,FechaRecibo,UsuarioId,Observaciones,FechaAnualidad)
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.NumeroAnualidad = t2.NumeroAnualidad)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.FechaVencimiento = t2.FechaVencimiento,
                           t1.Valor = t2.Valor,
                           t1.Recibo = t2.Recibo,
                           t1.FechaRecibo = t2.FechaRecibo,
                           t1.UsuarioId = t2.UsuarioId,
                           t1.Observaciones = t2.Observaciones,
                           t1.FechaAnualidad = t2.FechaAnualidad
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId,NumeroAnualidad,EsRenovacion,FechaVencimiento,Valor,Recibo,FechaRecibo,UsuarioId,Observaciones,FechaAnualidad) 
                           values(t2.ExpedienteId, t2.NumeroAnualidad, t2.EsRenovacion, t2.FechaVencimiento, t2.Valor, t2.Recibo, t2.FechaRecibo, t2.UsuarioId, t2.Observaciones, t2.FechaAnualidad);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NumeroAnualidad = numeroAnualidad,
                                EsRenovacion = esrenovacion == "S",
                                FechaVencimiento = fVencimiento,
                                Valor = valor,
                                Recibo = recibo,
                                FechaRecibo = fVencimiento,
                                UsuarioId = 0,
                                Observaciones = observaciones,
                                FechaAnualidad = fAnualidad
                            }
                        );
                    }
                    else
                    {
                        log.Error("Anualidades: " + expedienteId);
                    }
                    log.console("Anualidades:" + idsolicitud);

                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Anualidad {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Anualidades


        /// DoctosExt
        public static void ImportDoctosExt()
        {
            string DBF_FileName = "doctosext";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromDoctosExt = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromDoctosExt, dbaseConn);

            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    string file = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string descripcion = dataReader.GetValue(4).ToString();
                    string usuario = dataReader.GetValue(5).ToString(); // PENDIENTE!!

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int cronologiaId = cnn.Query<int>("SELECT TOP 1 Id FROM [dbo].[Cronologia] WHERE ExpedienteId = @expedienteId AND Fecha=@fecha", new { expedienteId = expedienteId, fecha = fecha }).FirstOrDefault();

                    if (expedienteId > 0 && cronologiaId > 0) //cronologiaId=0 resolucion sin cronologia
                    {                        
                        cnn.Execute(@"MERGE INTO patentes.ResExternas as t1 
                        using(SELECT @CronologiaId, @Docto, @Descripcion) AS t2 
                                   (CronologiaId, Docto, Descripcion)
                        ON (t1.CronologiaId = t2.CronologiaId)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.Docto = t2.Docto,
                           t1.Descripcion = t2.Descripcion
                        WHEN NOT MATCHED THEN
                           insert (CronologiaId, Docto, Descripcion)
                           values(t2.CronologiaId, t2.Docto, t2.Descripcion);",
                                new
                                {
                                    CronologiaId = cronologiaId,
                                    Docto = file,
                                    Descripcion = descripcion
                                }
                            );
                    }
                    else
                    {
                        log.Error("doctosext: " + expedienteId);
                    }
                    log.console("doctosext:" + idsolicitud);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Docto ext {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END DoctosExt


        /// Inventores
        public static void ImportInventores()
        {
            string DBF_FileName = "inventores";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromInventores = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromInventores, dbaseConn);

            var dataReader = command.ExecuteReader();
            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    string nombre = dataReader.GetString(2);
                    string direccion = dataReader.GetString(3);
                    string ciudad = dataReader.GetString(4);
                    string pais = dataReader.GetString(5);
                    string telefono = dataReader.GetString(6);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    if (expedienteId > 0 && paisId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.Inventores as t1 
                        using(SELECT @ExpedienteId, @Nombre, @Direccion, @Ciudad, @PaisId, @Telefono) AS t2 
                                   (ExpedienteId, Nombre, Direccion, Ciudad, PaisId, Telefono)
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.Nombre = t2.Nombre)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.Direccion = t2.Direccion,
                           t1.Ciudad = t2.Ciudad,
                           t1.PaisId = t2.PaisId,
                           t1.Telefono = t2.Telefono
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, Nombre, Direccion, Ciudad, PaisId, Telefono)
                           values(t2.ExpedienteId, t2.Nombre, t2.Direccion, t2.Ciudad, t2.PaisId, t2.Telefono);",
                                new
                                {
                                    ExpedienteId = expedienteId,
                                    Nombre = nombre,
                                    Direccion = direccion,
                                    Ciudad = ciudad,
                                    PaisId = paisId,
                                    Telefono = telefono
                                }
                            );
                    }
                    else
                    {
                        log.Error("Inventor :" + idsolicitud);
                    }
                    log.console("Inventores:" + idsolicitud);

                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Inventor {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Inventores

        /// IPC
        /// add another column char of two and move indice to that column then remove all dirty data
        public static void ImportIPC() //Create index column from indice char of 2, -- REMOVE SET FILTER TO index="**"
        {
            string DBF_FileName = "ipc";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromIPC = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud, index",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromIPC, dbaseConn);

            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);

                    int indice = int.Parse(dataReader.GetValue(6).ToString());
                    string clas_int = dataReader.GetString(3);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    if (expedienteId > 0 && !string.IsNullOrEmpty(clas_int.Trim()))
                    {
                        cnn.Execute(@"MERGE INTO patentes.IPC as t1 
                        using(SELECT @ExpedienteId, @Indice, @Classificacion) AS t2 
                                   (ExpedienteId, Indice, Classificacion)
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.Indice = t2.Indice)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.Classificacion = t2.Classificacion
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, Indice, Classificacion)
                           values(t2.ExpedienteId, t2.Indice, t2.Classificacion);",
                                new
                                {
                                    ExpedienteId = expedienteId,
                                    Indice = indice,
                                    Classificacion = clas_int
                                }
                            );
                    }
                    else
                    {
                        log.Error(idsolicitud);
                    }

                    log.console("ipc:" + idsolicitud);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("IPC {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END IPC


        /// Prioridades
        public static void ImportPrioridades()
        {
            string DBF_FileName = "PRIORIDAD";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromPrioridad = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromPrioridad, dbaseConn);

            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);

                    string pais = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string tipo_referencia = dataReader.GetString(4);
                    string solicitudp = dataReader.GetString(5);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();


                    if (expedienteId > 0 && paisId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.Prioridades as t1 
                            using(SELECT @ExpedienteId, @PaisId, @Fecha, @Tipo_referencia, @SolicitudP) AS t2 
                                       (ExpedienteId, PaisId, Fecha, Tipo_referencia, SolicitudP)
                            ON (t1.ExpedienteId = t2.ExpedienteId AND t1.Fecha = t2.Fecha AND t1.PaisId = t2.PaisId)
                            WHEN MATCHED THEN
                            UPDATE SET
                               t1.Tipo_referencia = t2.Tipo_referencia,
                               t1.SolicitudP = t2.SolicitudP
                            WHEN NOT MATCHED THEN
                               insert (ExpedienteId, PaisId, Fecha, Tipo_referencia, SolicitudP)
                               values(t2.ExpedienteId, t2.PaisId, t2.Fecha, t2.Tipo_referencia, t2.SolicitudP);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                PaisId = paisId,
                                Fecha = fecha,
                                Tipo_referencia = tipo_referencia,
                                SolicitudP = solicitudp
                            }
                        );
                    }
                    else{
                        log.Error("Prioridad:" + idsolicitud);
                    }

                    log.console("Prioridad:" + idsolicitud);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Prioridad {0}, {1} tiene errores", tipo_registro, idsolicitud));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Prioridades


        /// Titulares
        public static void ImportTitulares()
        {
            string DBF_FileName = "titular";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idtitular = string.Empty;
            string nombre = string.Empty;
            string pais = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idtitular = dataReader.GetValue(0).ToString();
                    nombre = dataReader.GetString(1);
                    pais = dataReader.GetString(2);
                    if (string.IsNullOrEmpty(pais))
                        pais = "GT";

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();

                    if (paisId > 0)
                    {                        
                        cnn.Execute(@"MERGE INTO patentes.TitularesEnPatentes as t1 
                            using(SELECT @Nombre, @PaisId, @tmpId) AS t2 
                                       (Nombre, PaisId, tmpId)
                            ON (t1.tmpId = t2.tmpId)
                            WHEN MATCHED THEN
                            UPDATE SET
                               t1.Nombre = t2.Nombre,
                               t1.PaisId = t2.PaisId
                            WHEN NOT MATCHED THEN
                               insert (Nombre, PaisId, tmpId)
                               values(t2.Nombre, t2.PaisId, t2.tmpId);",
                                new
                                {
                                    Nombre = nombre,
                                    PaisId = paisId,
                                    tmpId = idtitular
                                }
                            );
                    }
                    else
                    {
                        log.Error("titular:" + nombre);
                    }
                    log.Error("titular:" + nombre);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("TitularesEnPatentes {0}, {1} tiene errores", idtitular, nombre));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END Titulares


        /// TitularesDeLaPatente
        public static void ImportTitularesDeLaPatente()
        {
            string DBF_FileName = "titulares_x_pat";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromTitulares_x_pat = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromTitulares_x_pat, dbaseConn);

            var dataReader = command.ExecuteReader();
            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;
            string idtitular = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    idtitular = dataReader.GetValue(2).ToString();

                    string direcciontit = dataReader.GetString(3);
                    string pais = dataReader.GetString(4);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();
                    int titularId = cnn.Query<int>("SELECT Id FROM patentes.TitularesEnPatentes WHERE tmpId = @tmpId", new { tmpId = idtitular }).FirstOrDefault();

                    if (expedienteId > 0 && titularId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.TitularesDeLaPatente as t1 
                            using(SELECT @ExpedienteId, @TitularId, @Direccion, @PaisId) AS t2 
                                       (ExpedienteId, TitularId, Direccion, PaisId)
                            ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TitularId = t2.TitularId)
                            WHEN MATCHED THEN
                            UPDATE SET
                               t1.Direccion = t2.Direccion,
                               t1.PaisId = t2.PaisId
                            WHEN NOT MATCHED THEN
                               insert (ExpedienteId, TitularId, Direccion, PaisId)
                               values(t2.ExpedienteId, t2.TitularId, t2.Direccion, t2.PaisId);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                TitularId = titularId,
                                Direccion = direcciontit,
                                PaisId = paisId
                            }
                        );
                    }
                    else{
                        log.Error("titulares_x_pat:" + idsolicitud);
                    }

                    log.console("titulares_x_pat:" + idsolicitud);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Titular de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, idtitular));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END TitularesDeLaPatente


        /// Titulos
        public static void ImportTitulos()
        {
            string DBF_FileName = "TITULOS";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromTitulos = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromTitulos, dbaseConn);

            var dataReader = command.ExecuteReader();
            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;
            string tipo_resolucion = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    tipo_resolucion = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string observaciones = dataReader.GetString(4);
                    DateTime fecha_anotacion = dataReader.GetDateTime(5);
                    DateTime fecha_consecion = dataReader.GetDateTime(6);
                    string plazo = dataReader.GetValue(7).ToString();
                    string clasificacion = dataReader.GetValue(8).ToString();
                    string flag = dataReader.GetValue(9).ToString();
                    string tipo_acuerdo = dataReader.GetValue(11).ToString();
                    string acuerdo = dataReader.GetValue(12).ToString();
                    DateTime fecha_acuerdo = dataReader.GetDateTime(13);

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.Titulos as t1 
                            using(SELECT @ExpedienteId, @TipoResolucion, @Fecha, @Observaciones, @FechaAnotaciones, @FechaConsecion, @Plazo, @Clasificacion, @Flag, @TipoDeAcuerdoId, @Acuerdo, @FechaAcuerdo) AS t2 
                            (ExpedienteId, TipoResolucion, Fecha, Observaciones, FechaAnotaciones, FechaConsecion, Plazo, Clasificacion, Flag, TipoDeAcuerdoId, Acuerdo, FechaAcuerdo)
                            ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TipoResolucion = t2.TipoResolucion AND t1.Fecha = t2.fecha)
                            WHEN MATCHED THEN
                            UPDATE SET                                
                                t1.Observaciones = t2.Observaciones,
                                t1.FechaAnotaciones = t2.FechaAnotaciones,
                                t1.FechaConsecion = t2.FechaConsecion,
                                t1.Plazo = t2.Plazo,
                                t1.Clasificacion = t2.Clasificacion,
                                t1.Flag = t2.Flag,
                                t1.TipoDeAcuerdoId = t2.TipoDeAcuerdoId,
                                t1.Acuerdo = t2.Acuerdo,
                                t1.FechaAcuerdo = t2.FechaAcuerdo
                            WHEN NOT MATCHED THEN
                               insert (ExpedienteId, TipoResolucion, Fecha, Observaciones, FechaAnotaciones, FechaConsecion, Plazo, Clasificacion, Flag, TipoDeAcuerdoId, Acuerdo, FechaAcuerdo)
                               values(t2.ExpedienteId, t2.TipoResolucion, t2.Fecha, t2.Observaciones, t2.FechaAnotaciones, t2.FechaConsecion, t2.Plazo, t2.Clasificacion, t2.Flag, t2.TipoDeAcuerdoId, t2.Acuerdo, t2.FechaAcuerdo);",
                                new
                                {

                                    ExpedienteId = expedienteId,
                                    TipoResolucion = tipo_resolucion,
                                    Fecha = fecha,
                                    Observaciones = observaciones,
                                    FechaAnotaciones = fecha_anotacion,
                                    FechaConsecion = fecha_consecion,
                                    Plazo = plazo,
                                    Clasificacion = clasificacion,
                                    Flag = flag,
                                    TipoDeAcuerdoId = tipo_acuerdo,
                                    Acuerdo = acuerdo,
                                    FechaAcuerdo = fecha_acuerdo
                                }
                            );
                    }
                    else
                    {
                        log.Error("Titulos:" + idsolicitud);
                    }

                    log.Error("Titulos:" + idsolicitud);

                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("Titulo de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, tipo_resolucion));
                    log.Error(exception.Message);
                }

            }
            dataReader.Close();
        } // END Titulos


        /// [Resoluciones]-Transacciones.dbf
        public static void ImportResoluciones()
        {
            string DBF_FileName = "transacciones";

            string initMain = Utils.getMigracionParam(cnn, "pat.main");
            string selectFromTran = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");


            OleDbCommand command = new OleDbCommand(selectFromTran, dbaseConn);

            var dataReader = command.ExecuteReader();

            string tipo_registro = string.Empty;
            string idsolicitud = string.Empty;
            string tipo_resolucion = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    tipo_registro = dataReader.GetValue(0).ToString();
                    idsolicitud = dataReader.GetString(1);
                    tipo_resolucion = dataReader.GetString(2);
                    DateTime fecha = dataReader.GetDateTime(3);
                    string observaciones = dataReader.GetString(4);
                    string choices = dataReader.GetString(5);
                    string otros = dataReader.GetString(6);
                    DateTime fechanotificacionobs = dataReader.GetDateTime(7);
                    DateTime fechapublicacion = dataReader.GetDateTime(8);
                    string hora = dataReader.GetValue(9).ToString();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=2 AND codigo like @codigo", new { codigo = tipo_registro + "%" }).FirstOrDefault();
                    
                    int expedienteId = Utils.getExpedienteIdDePatentes(cnn, idsolicitud, tipoDeRegistroId);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO patentes.Resoluciones as t1 
                            using(SELECT @ExpedienteId, @TipoResolucion, @Fecha, @Observaciones, @Opciones, @Otros, @FechaNotificacion, @FechaPublicacion, @Hora) AS t2 
                            (ExpedienteId, TipoResolucion, Fecha, Observaciones, Opciones, Otros, FechaNotificacion, FechaPublicacion, Hora)
                            ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TipoResolucion = t2.TipoResolucion AND t1.Fecha = t2.fecha)
                            WHEN MATCHED THEN
                            UPDATE SET
                                t1.Observaciones = t2.Observaciones,
                                t1.Opciones = t2.Opciones,
                                t1.Otros = t2.Otros,
                                t1.FechaNotificacion = t2.FechaNotificacion,
                                t1.FechaPublicacion = t2.FechaPublicacion,
                                t1.Hora = t2.Hora
                            WHEN NOT MATCHED THEN
                               insert (ExpedienteId, TipoResolucion, Fecha, Observaciones, Opciones, Otros, FechaNotificacion, FechaPublicacion, Hora)
                               values(t2.ExpedienteId, t2.TipoResolucion, t2.Fecha, t2.Observaciones, t2.Opciones, t2.Otros, t2.FechaNotificacion, t2.FechaPublicacion, t2.Hora);",
                                    new
                                    {

                                        ExpedienteId = expedienteId,
                                        TipoResolucion = tipo_resolucion,
                                        Fecha = fecha,
                                        Observaciones = observaciones,
                                        Opciones = choices,
                                        Otros = otros,
                                        FechaNotificacion = fechanotificacionobs,
                                        FechaPublicacion = fechapublicacion,
                                        Hora = hora
                                    }
                                );
                    }
                    else
                    {
                        log.Error("transacciones:" + idsolicitud);
                    }

                    log.Error("transacciones:" + idsolicitud);
                } //end try
                catch (Exception exception)
                {
                    log.Error(string.Format("transacciones de la patente {0}, {1}, {2} tiene errores", tipo_registro, idsolicitud, tipo_resolucion));
                    log.Error(exception.Message);
                }
            }
            dataReader.Close();
        } // END [Resoluciones]-Transacciones

    }
}
