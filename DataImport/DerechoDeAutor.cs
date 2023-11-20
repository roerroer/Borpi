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
    public class DerechoDeAutor
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
            string FilePath = ConfigurationManager.AppSettings["legacyDA"];

            var conn = new OleDbConnection(string.Format(dbaseConnString, FilePath));            
            conn.Open();
            return conn;

        }

        private static SqlConnection cnn = GetOpenConnection();
        private static OleDbConnection dbaseConn = GetdbaseOpenConnection();

        static DerechoDeAutor()
        {
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
            //    string descripcion = dataReader.GetString(1);

            //    int expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Estatus([Codigo],[Descripcion],[ModuloId])
            //        VALUES( @Codigo, @Descripcion, @ModuloId);
            //        SELECT SCOPE_IDENTITY() AS [EstatusId]; ",
            //        new 
            //        { 
            //            Codigo=idstatus, 
            //            Descripcion=descripcion,  
            //            ModuloId = 3
            //        }
            //        ).Single();
            //}
            //dataReader.Close();
        }

        public static void ImportFormularios()
        {
            //string DBF_FileName = "formularios";
            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            //var dataReader = command.ExecuteReader();
            //while (dataReader.Read())
            //{              
            //    string codigo = dataReader.GetValue(0).ToString();
            //    string considerando = dataReader.GetString(2);
            //    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=3 AND codigo like @codigo", new { codigo = codigo + "%" }).FirstOrDefault();

            //    if (tipoDeRegistroId > 0)
            //    {
            //        cnn.Query<int>(@"INSERT INTO da.Formularios([TipoDeRegistroId],[Considerando])
            //        VALUES( @TipoDeRegistroId, @Considerando);",
            //            new
            //            {
            //                TipoDeRegistroId = tipoDeRegistroId,
            //                Considerando = considerando
            //            }
            //            );
            //    }
            //    else
            //    {
            //        log.Error(string.Format("Formulario no encontrado {0}", codigo));
            //    }
            //}
            //dataReader.Close();
        }

        // Expedientes
        public static void ImportExpedientes()
        {
            string DBF_FileName = "datosobra";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromDatosObra = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromDatosObra, dbaseConn);
            log.Error("agregar FECHAP2 a datosobra con tipo fecha y llenarlo con el campo fecha REPLACE fechap2 WITH CTOD(fechap) ALL");            

            var dataReader = command.ExecuteReader();

            string idsolicitud = String.Empty;
            string idstatus = String.Empty;
            string idtipo = String.Empty;
            DateTime timeNow = DateTime.Now;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    idstatus = dataReader.GetString(1);
                    idtipo = dataReader.GetString(2);

                    DateTime? fechasolicitud = dataReader.GetDateTime(3);
                    string horaingreso = dataReader.GetString(4);
                    string titulo = dataReader.GetValue(5).ToString();
                    string traduccion = dataReader.GetValue(6).ToString();

                    int paginas = int.Parse(dataReader.GetValue(7).ToString());

                    string formato = dataReader.GetValue(8).ToString();
                    string lugared = dataReader.GetValue(9).ToString();

                    DateTime? fechaed = dataReader.GetDateTime(10);

                    string lugarp = dataReader.GetValue(11).ToString();
                    string fechap = dataReader.GetValue(12).ToString();
                    string editor = dataReader.GetValue(13).ToString();
                    string yearcreacion = dataReader.GetValue(14).ToString();
                    string paisorigen = dataReader.GetValue(15).ToString();
                    bool obra1 = dataReader[16] as int? == 1 ? true : false;
                    bool obra2 = dataReader[17] as int? == 1 ? true : false;
                    bool obra3 = dataReader[18] as int? == 1 ? true : false;
                    bool obra4 = dataReader[19] as int? == 1 ? true : false;
                    bool obra5 = dataReader[20] as int? == 1 ? true : false;
                    bool obra6 = dataReader[21] as int? == 1 ? true : false;
                    bool obra7 = dataReader[22] as int? == 1 ? true : false;

                    string otraclasifica = dataReader.GetValue(23).ToString();
                    string versionesautoriza = dataReader.GetValue(24).ToString();
                    string otrainfoqueidentifique = dataReader.GetValue(25).ToString();
                    string soporte = dataReader.GetValue(26).ToString();
                    string usuario = dataReader.GetValue(27).ToString();
                    int registro = int.Parse(dataReader.GetValue(28).ToString());
                    string libro = dataReader.GetValue(29).ToString();
                    int tomo = int.Parse(dataReader.GetValue(30).ToString());
                    int folio = int.Parse(dataReader.GetValue(31).ToString());
                    
                    string fechap2 = dataReader.GetValue(32).ToString();
                    
                    List<String> timex = new List<String>();

                    TimeSpan time = Utils.parseTime(horaingreso, DBF_FileName + "(horaingreso)", idsolicitud);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = idstatus }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE ModuloId=3 AND codigo like @codigo", new { codigo = idtipo + "%" }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = paisorigen }).FirstOrDefault();

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    expedienteId = cnn.Query<int>(@"MERGE INTO dbo.Expedientes as t1 
                        using(SELECT @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId, @FechaActualizacion) AS t2 
                                   (ModuloId, TipoDeRegistroId, Numero, FechaDeSolicitud, Hora, EstatusId, FechaDeEstatus, LeyId, FechaActualizacion)
                        ON (t1.TipoDeRegistroId = t2.TipoDeRegistroId AND t1.Numero = t2.Numero AND t1.ModuloId = 3)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.FechaDeSolicitud = t2.FechaDeSolicitud,
                           t1.Hora = t2.Hora,
                           t1.EstatusId = t2.EstatusId,
                           t1.FechaDeEstatus = t2.FechaDeEstatus,
                           t1.LeyId = t2.LeyId,
                           t1.FechaActualizacion = t2.FechaActualizacion
                        WHEN NOT MATCHED THEN
                           insert (ModuloId, TipoDeRegistroId, Numero, FechaDeSolicitud, Hora, EstatusId, FechaDeEstatus, LeyId, FechaActualizacion) 
                           values(t2.ModuloId, t2.TipoDeRegistroId, t2.Numero, t2.FechaDeSolicitud, t2.Hora, t2.EstatusId, t2.FechaDeEstatus, t2.LeyId, t2.FechaActualizacion);
                           SELECT IIF(@ExpedienteId=0, SCOPE_IDENTITY(), @ExpedienteId) AS [expedienteId];",
                            new
                            {
                                ExpedienteId = expedienteId,
                                ModuloId = 3, // Derecho de autor 
                                TipoDeRegistroId = tipoDeRegistroId,
                                Numero = idsolicitud,
                                FechaDeSolicitud = fechasolicitud,
                                Hora = time,
                                EstatusId = statusId,
                                FechaDeEstatus = fechasolicitud, // actualizar con dataimport cronologia!!
                                LeyId = "2",
                                FechaActualizacion = timeNow,
                            }
                        ).Single(); ;

                    if (expedienteId > 0) {
                        cnn.Execute(@"MERGE INTO da.DerechoDeAutor as t1 
                        using(SELECT @ExpedienteId, @Titulo, @Traduccion, @Paginas, @Formato, @LugarEdicion, @FechaEdicion, @LugarEraPublicacion, 
                                @FechaPublicacion, @Editor, @AnioCreacion, @PaisOrigenId, @EsInedita, @EsPublicada, @EsOriginaria, @EsDerivada, @EsIndividual,
                                @EsColectiva, @EsEnColaboracion, @OtraClasificacionAplicable, @VersionesAutorizadas, @OtraInfoQueIdentifique, @SoporteMaterial,
                                @Registro, @Libro, @Tomo,@Folio) AS t2
                                   (ExpedienteId, Titulo, Traduccion, Paginas, Formato, LugarEdicion, FechaEdicion, LugarEraPublicacion, 
                                    FechaPublicacion, Editor, AnioCreacion, PaisOrigenId, EsInedita, EsPublicada, EsOriginaria, EsDerivada, EsIndividual, 
                                    EsColectiva, EsEnColaboracion, OtraClasificacionAplicable, VersionesAutorizadas, OtraInfoQueIdentifique, SoporteMaterial, 
                                    Registro, Libro, Tomo, Folio)
                        ON (t1.ExpedienteId = t2.ExpedienteId)
                        WHEN MATCHED THEN
                        UPDATE SET
                            t1.Titulo = t2.Titulo,
                            t1.Traduccion = t2.Traduccion,
                            t1.Paginas = t2.Paginas,
                            t1.Formato = t2.Formato,
                            t1.LugarEdicion = t2.LugarEdicion,
                            t1.FechaEdicion = t2.FechaEdicion,
                            t1.LugarEraPublicacion = t2.LugarEraPublicacion,
                            t1.FechaPublicacion = t2.FechaPublicacion,
                            t1.Editor = t2.Editor,
                            t1.AnioCreacion = t2.AnioCreacion,
                            t1.PaisOrigenId = t2.PaisOrigenId,
                            t1.EsInedita = t2.EsInedita,
                            t1.EsPublicada = t2.EsPublicada,
                            t1.EsOriginaria = t2.EsOriginaria,
                            t1.EsDerivada = t2.EsDerivada,
                            t1.EsIndividual = t2.EsIndividual,
                            t1.EsColectiva = t2.EsColectiva,
                            t1.EsEnColaboracion = t2.EsEnColaboracion,
                            t1.OtraClasificacionAplicable = t2.OtraClasificacionAplicable,
                            t1.VersionesAutorizadas = t2.VersionesAutorizadas,
                            t1.OtraInfoQueIdentifique = t2.OtraInfoQueIdentifique,
                            t1.SoporteMaterial = t2.SoporteMaterial,
                            t1.Registro = t2.Registro,
                            t1.Libro = t2.Libro,
                            t1.Tomo = t2.Tomo,
                            t1.Folio = t2.Folio
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, Titulo, Traduccion, Paginas, Formato, LugarEdicion, FechaEdicion, LugarEraPublicacion, 
                                    FechaPublicacion, Editor, AnioCreacion, PaisOrigenId, EsInedita, EsPublicada, EsOriginaria, EsDerivada, EsIndividual, 
                                    EsColectiva, EsEnColaboracion, OtraClasificacionAplicable, VersionesAutorizadas, OtraInfoQueIdentifique, SoporteMaterial, 
                                    Registro, Libro, Tomo, Folio) 
                           values(t2.ExpedienteId, t2.Titulo, t2.Traduccion, t2.Paginas, t2.Formato, t2.LugarEdicion, t2.FechaEdicion, t2.LugarEraPublicacion, 
                                    t2.FechaPublicacion, t2.Editor, t2.AnioCreacion, t2.PaisOrigenId, t2.EsInedita, t2.EsPublicada, t2.EsOriginaria, t2.EsDerivada, t2.EsIndividual, 
                                    t2.EsColectiva, t2.EsEnColaboracion, t2.OtraClasificacionAplicable, t2.VersionesAutorizadas, t2.OtraInfoQueIdentifique, t2.SoporteMaterial, 
                                    t2.Registro, t2.Libro, t2.Tomo, t2.Folio);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Titulo = titulo,
                                Traduccion = traduccion,
                                Paginas = paginas,
                                Formato = formato,
                                LugarEdicion = lugared,
                                FechaEdicion = fechaed,
                                LugarEraPublicacion = lugarp,
                                FechaPublicacion = fechap2,
                                Editor = editor,
                                AnioCreacion = yearcreacion,
                                PaisOrigenId = paisId,
                                EsInedita = obra1,
                                EsPublicada = obra2,
                                EsOriginaria = obra3,
                                EsDerivada = obra4,
                                EsIndividual = obra5,
                                EsColectiva = obra6,
                                EsEnColaboracion = obra7,
                                OtraClasificacionAplicable = otraclasifica,
                                VersionesAutorizadas = versionesautoriza,
                                OtraInfoQueIdentifique = otrainfoqueidentifique,
                                SoporteMaterial = soporte,
                                Registro = registro,
                                Libro = libro,
                                Tomo = tomo,
                                Folio = folio
                            }
                        );
                    }
                    log.console(string.Format("expediente {0}:", idsolicitud));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0}, {1} tiene errores", idsolicitud, idtipo));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END Expedientes


        /// Cronologia
        public static void ImportCrono()
        {
            DateTime todayMinus5 = Utils.getInitDateForCrono();
            string DBF_FileName = "cronologia";

            string allCrono = Utils.getMigracionParam(cnn, "da.crono");

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

            string idsolicitud = string.Empty;
            DateTime? fecha = null;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetString(0);
                    fecha = dataReader.GetDateTime(1);
                    string idstatus_act = dataReader.GetString(2);
                    string idstatus_ant = dataReader.GetString(3);
                    string referencia = dataReader.GetString(4);
                    string usuario = dataReader.GetString(5);
                    string observaciones = dataReader.GetString(6);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = idstatus_act }).FirstOrDefault();

                    if (expedienteId > 0)
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
                                 UsuarioIniciales = usuario
                             }
                            );
                    }

                    log.console(string.Format("Crono {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Cronologia {0}, {1} tiene errores", idsolicitud, fecha));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
            log.Error("DAutor cronologia imported");
        }

        /// artliterarias - LiterariasyArtisticas
        public static void ImportLiterariasyArtisticas()
        {
            string DBF_FileName = "artliterarias";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromArtliterarias = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");
        
            OleDbCommand command = new OleDbCommand(selectFromArtliterarias, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string nombreed = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    nombreed = dataReader.GetString(1);
                    string direccioned = dataReader.GetString(2);
                    string nombreimp = dataReader.GetString(3);
                    string direccionimp = dataReader.GetString(4);
                    string nedicion = dataReader.GetString(5);
                    string tamano = dataReader.GetString(6);
                    string clasifica = dataReader.GetString(7);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.LiterariasyArtisticas as t1 
                        using(SELECT @ExpedienteId, @NombreDelEditor, @DireccionEditor, @NombreImprenta, @DireccionImprenta, @Edicion, @Tamano, @Clasificacion) AS t2 
                                   (ExpedienteId, NombreDelEditor, DireccionEditor, NombreImprenta, DireccionImprenta, Edicion, Tamano, Clasificacion)
                        ON (t1.ExpedienteId = t2.ExpedienteId)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.NombreDelEditor = t2.NombreDelEditor,
                           t1.DireccionEditor = t2.DireccionEditor,
                           t1.NombreImprenta = t2.NombreImprenta,
                           t1.DireccionImprenta = t2.DireccionImprenta,
                           t1.Tamano = t2.Tamano,
                           t1.Clasificacion = t2.Clasificacion
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, NombreDelEditor, DireccionEditor, NombreImprenta, DireccionImprenta, Edicion, Tamano, Clasificacion) 
                           values(t2.ExpedienteId, t2.NombreDelEditor, t2.DireccionEditor, t2.NombreImprenta, t2.DireccionImprenta, t2.Edicion, t2.Tamano, t2.Clasificacion);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreDelEditor = nombreed,
                                DireccionEditor = direccioned,
                                NombreImprenta = nombreimp,
                                DireccionImprenta = direccionimp,
                                Edicion = nedicion,
                                Tamano = tamano,
                                Clasificacion = clasifica
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("artliterarias not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("artliterarias {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("LiterariasyArtisticas {0}, {1} tiene errores", idsolicitud, nombreed));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END artliterarias - LiterariasyArtisticas

        /// AudiovisualAutores > audio_autoro
        public static void ImportAudiovisualAutores()
        {
            string DBF_FileName = "audio_autoro";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromAudioAutoro = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAudioAutoro, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string autor_obra = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    autor_obra = dataReader.GetString(1);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0) {
                        cnn.Execute(@"MERGE INTO da.AudiovisualAutores as t1 
                                using(SELECT @ExpedienteId, @NombreAutor) AS t2 
                                           (ExpedienteId, NombreAutor)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.NombreAutor = t2.NombreAutor
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, NombreAutor) 
                                   values(t2.ExpedienteId, t2.NombreAutor);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreAutor = autor_obra
                            }
                        );
                    } 
                    else
                    {
                        log.Error(string.Format("audio_autoro not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("audio_autoro {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("AudiovisualAutores {0}, {1} tiene errores", idsolicitud, autor_obra));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } // END AudiovisualAutores > audio_autoro


        /// AporteAudiovisual ->audioaporte.dbf
        public static void ImportAporteAudiovisual()
        {
            string DBF_FileName = "audioaporte";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromAudioAporte = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAudioAporte, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string director = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    director = dataReader.GetString(1);
                    string genero = dataReader.GetString(2);
                    string clase = dataReader.GetString(3);
                    string metraje = dataReader.GetString(4);
                    string duracion = dataReader.GetString(5);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.AporteAudiovisual as t1 
                            using(SELECT @ExpedienteId, @Director, @Genero, @Clase, @Metraje, @Duracion) AS t2 
                                        (ExpedienteId, Director, Genero, Clase, Metraje, Duracion)
                            ON (t1.ExpedienteId = t2.ExpedienteId)
                            WHEN MATCHED THEN
                            UPDATE SET
                                t1.Director = t2.Director,
                                t1.Genero = t2.Genero,
                                t1.Clase = t2.Clase,
                                t1.Metraje = t2.Metraje,
                                t1.Duracion = t2.Duracion
                            WHEN NOT MATCHED THEN
                                insert (ExpedienteId, Director, Genero, Clase, Metraje, Duracion) 
                                values(t2.ExpedienteId, t2.Director, t2.Genero, t2.Clase, t2.Metraje, t2.Duracion);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Director = director,
                                Genero = genero,
                                Clase = clase,
                                Metraje = metraje,
                                Duracion = duracion
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("AporteAudiovisual not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("AporteAudiovisual {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("AporteAudiovisual {0}, {1} tiene errores", idsolicitud, director));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END AporteAudiovisual ->audioaporte.dbf


        /// ComposicionAutores > audiocomp.dbf 
        public static void ImportComposicionAutores()
        {
            string DBF_FileName = "audiocomp";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromAudioComp = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAudioComp, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string autor_compo = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    autor_compo = dataReader.GetString(1);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.ComposicionAutores as t1 
                                using(SELECT @ExpedienteId, @NombreAutor) AS t2 
                                           (ExpedienteId, NombreAutor)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.NombreAutor = t2.NombreAutor
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, NombreAutor) 
                                   values(t2.ExpedienteId, t2.NombreAutor);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreAutor = autor_compo
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("audiocomp not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("audiocomp {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("audiocomp {0}, {1} tiene errores", idsolicitud, autor_compo));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END ComposicionAutores > audiocomp.dbf


        /// GuionAutores - audioguion
        public static void ImportGuionAutores()
        {
            string DBF_FileName = "audioguion";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromAudioGuion = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAudioGuion, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string autor = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    autor = dataReader.GetString(1);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.GuionAutores as t1 
                                using(SELECT @ExpedienteId, @NombreAutor) AS t2 
                                           (ExpedienteId, NombreAutor)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.NombreAutor = t2.NombreAutor
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, NombreAutor) 
                                   values(t2.ExpedienteId, t2.NombreAutor);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreAutor = autor
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("audioguion not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("audioguion {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("audioguion {0}, {1} tiene errores", idsolicitud, autor));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END GuionAutores - audioguion

        /// Autores
        public static void ImportAutores()
        {
            string DBF_FileName = "autores";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromAutores = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromAutores, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string nombre = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    nombre = dataReader.GetString(1);

                    int edad = int.Parse(dataReader.GetValue(2).ToString());

                    string estadocivil = dataReader.GetValue(3).ToString();
                    string profesion = dataReader.GetValue(4).ToString();
                    string domicilio = dataReader.GetValue(5).ToString();
                    string nacionalidad = dataReader.GetValue(6).ToString();
                    string identificacion = dataReader.GetValue(7).ToString();
                    string lugarnoti = dataReader.GetValue(8).ToString();
                    string tel = dataReader.GetValue(9).ToString();
                    string fax = dataReader.GetValue(10).ToString();
                    string email = dataReader.GetValue(11).ToString();

                    DateTime? fechanac = dataReader.GetDateTime(12);
                    DateTime? fechadef = dataReader.GetDateTime(13);
                    string datosbibliograficos = dataReader.GetValue(14).ToString();

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.Autores as t1 
                                using(SELECT @ExpedienteId, @NombreAutor, @Edad, @EstadoCivil, @Profesion, @Domicilio, @Nacionalidad, @Identificacion, 
                                             @LugarNotificacion, @Telefono, @Fax, @Email, @FechaNacimiento, @FechaDef, @DatosBibliograficos) AS t2 
                                           (ExpedienteId, NombreAutor, Edad, EstadoCivil, Profesion, Domicilio, Nacionalidad, Identificacion, 
                                            LugarNotificacion, Telefono, Fax, Email, FechaNacimiento, FechaDef, DatosBibliograficos)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                    t1.NombreAutor = t2.NombreAutor,
                                    t1.Edad = t2.Edad,
                                    t1.EstadoCivil = t2.EstadoCivil,
                                    t1.Profesion = t2.Profesion,
                                    t1.Domicilio = t2.Domicilio,
                                    t1.Nacionalidad = t2.Nacionalidad,
                                    t1.Identificacion = t2.Identificacion,
                                    t1.LugarNotificacion = t2.LugarNotificacion,
                                    t1.Telefono = t2.Telefono,
                                    t1.Fax = t2.Fax,
                                    t1.Email = t2.Email,
                                    t1.FechaNacimiento = t2.FechaNacimiento,
                                    t1.FechaDef = t2.FechaDef,
                                    t1.DatosBibliograficos = t2.DatosBibliograficos
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, NombreAutor, Edad, EstadoCivil, Profesion, Domicilio, Nacionalidad, Identificacion, 
                                            LugarNotificacion, Telefono, Fax, Email, FechaNacimiento, FechaDef, DatosBibliograficos)
                                   values(t2.ExpedienteId, t2.NombreAutor, t2.Edad, t2.EstadoCivil, t2.Profesion, t2.Domicilio, t2.Nacionalidad, t2.Identificacion, 
                                            t2.LugarNotificacion, t2.Telefono, t2.Fax, t2.Email, t2.FechaNacimiento, t2.FechaDef, t2.DatosBibliograficos);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreAutor = nombre,
                                Edad = edad,
                                EstadoCivil = estadocivil,
                                Profesion = profesion,
                                Domicilio = domicilio,
                                Nacionalidad = nacionalidad,
                                Identificacion = identificacion,
                                LugarNotificacion = lugarnoti,
                                Telefono = tel,
                                Fax = fax,
                                Email = email,
                                FechaNacimiento = fechanac,
                                FechaDef = fechadef,
                                DatosBibliograficos = datosbibliograficos
                            }
                        );
                    }
                    else {
                        log.Error(string.Format("autores not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("autores {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Autores {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END Autores


        /// FonogramaArtistas - fono_art
        public static void ImportFonogramaArtistas()
        {
            string DBF_FileName = "fono_art";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromFonoArt = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromFonoArt, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string autor = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    autor = dataReader.GetString(1);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.FonogramaArtistas as t1 
                                using(SELECT @ExpedienteId, @NombreArtista) AS t2 
                                           (ExpedienteId, NombreArtista)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.NombreArtista = t2.NombreArtista
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, NombreArtista) 
                                   values(t2.ExpedienteId, t2.NombreArtista);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                NombreArtista = autor
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("fono_art not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("fono_art {0}:", idsolicitud));

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("FonogramaArtistas {0}, {1} tiene errores", idsolicitud, autor));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END FonogramaArtistas - fono_art

        /// FonogramaTituloDeObras - fono_aut_ob
        public static void ImportFonogramaTituloDeObras()
        {
            string DBF_FileName = "fono_aut_ob";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromFonoAutOb = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromFonoAutOb, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string titulo_obra = string.Empty;
            string autor = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    titulo_obra = dataReader.GetString(1);
                    autor = dataReader.GetString(2);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.FonogramaTituloDeObras as t1 
                                using(SELECT @ExpedienteId, @TituloObra, @NombreAutor) AS t2 
                                     (ExpedienteId, TituloObra, NombreAutor)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                    t1.TituloObra = t2.TituloObra,
                                    t1.NombreAutor = t2.NombreAutor
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, TituloObra, NombreAutor) 
                                   values(t2.ExpedienteId, t2.TituloObra, t2.NombreAutor);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                TituloObra = titulo_obra,
                                NombreAutor = autor,
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("fono_aut_ob not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("fono_aut_ob {0}:", idsolicitud));

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("FonogramaTituloDeObras {0}, {1} tiene errores", idsolicitud, titulo_obra));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END FonogramaTituloDeObras - fono_aut_ob


        ///  Templates - msword
        public static void ImportTemplates()
        {
            string DBF_FileName = "msword";
            OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName, dbaseConn);

            var dataReader = command.ExecuteReader();
            string estatusId = string.Empty;
            string descripcion = string.Empty;
            string docto = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    estatusId = dataReader.GetValue(0).ToString();
                    descripcion = dataReader.GetString(1);
                    docto = dataReader.GetString(2);
                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE ModuloId=3 AND Codigo = @codigo", new { codigo = estatusId }).FirstOrDefault();

                    if (statusId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.Templates as t1 
                            using(SELECT @EstatusId, @Contenido) AS t2 
                                        (EstatusId, Contenido)
                            ON (t1.EstatusId = t2.EstatusId)
                            WHEN MATCHED THEN
                            UPDATE SET
                                t1.Contenido = t2.Contenido
                            WHEN NOT MATCHED THEN
                                insert (EstatusId, Contenido) 
                                values(t2.EstatusId, t2.Contenido);",
                            new
                            {
                                EstatusId = statusId,
                                Contenido = docto
                            }
                        );
                    }

                } //end try
                catch (Exception exception)
                {
                    log.Error(exception.Message);
                    log.Error(estatusId);
                }
            }
            dataReader.Close();
        } // END Templates - msword


        //ObrasMusicalesyEscenicas > obrasm
        public static void ImportObrasMusicalesyEscenicas()
        {
            string DBF_FileName = "obrasm";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromObrasm = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromObrasm, dbaseConn);

            var dataReader = command.ExecuteReader();
            string idsolicitud = string.Empty;
            string partitura = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    partitura = dataReader.GetString(1);
                    string letra = dataReader.GetString(2);
                    string genero = dataReader.GetString(3);
                    bool autor = dataReader[4] as int? == 1 ? true : false;
                    bool musica = dataReader[5] as int? == 1 ? true : false;
                    bool comercial = dataReader[6] as int? == 1 ? true : false;
                    string claseogen = dataReader.GetString(7);
                    string duracion = dataReader.GetString(8);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.ObrasMusicalesyEscenicas as t1 
                                using(SELECT @ExpedienteId, @Partitura, @Letra, @Genero, @EsAutor, @EsMusica, @EsComercial, @ClaseGenero, @Duracion) AS t2 
                                           (ExpedienteId, Partitura, Letra, Genero, EsAutor, EsMusica, EsComercial, ClaseGenero, Duracion )
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.Partitura = t2.Partitura,
                                   t1.Letra = t2.Letra,
                                   t1.Genero = t2.Genero,
                                   t1.EsAutor = t2.EsAutor,
                                   t1.EsMusica = t2.EsMusica,
                                   t1.EsComercial = t2.EsComercial,
                                   t1.ClaseGenero = t2.ClaseGenero,
                                   t1.Duracion = t2.Duracion
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, Partitura, Letra, Genero, EsAutor, EsMusica, EsComercial, ClaseGenero, Duracion) 
                                   values(t2.ExpedienteId, t2.Partitura, t2.Letra, t2.Genero, t2.EsAutor, t2.EsMusica, t2.EsComercial, t2.ClaseGenero, t2.Duracion);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Partitura = partitura,
                                Letra = letra,
                                Genero = genero,
                                EsAutor = autor,
                                EsMusica = musica,
                                EsComercial = comercial,
                                ClaseGenero = claseogen,
                                Duracion = duracion
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("obrasm not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("obrasm {0}:", idsolicitud));

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("obrasm {0}, {1} tiene errores", idsolicitud, partitura));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } // END ObrasMusicalesyEscenicas - obrasm


        // Productores - productor.dbf
        public static void ImportProductores()
        {
            string DBF_FileName = "productor";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromProductor = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromProductor, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string nombre = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    nombre = dataReader.GetString(1);

                    int edad = int.Parse(dataReader.GetValue(2).ToString());
                    string estadocivil = dataReader.GetString(3);
                    string profesion = dataReader.GetString(4);
                    string nacionalidad = dataReader.GetString(5);
                    string domicilio = dataReader.GetString(6);
                    string tel = dataReader.GetString(7);
                    string fax = dataReader.GetString(8);
                    string email = dataReader.GetString(9);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.Productores as t1 
                                using(SELECT @ExpedienteId, @Nombre, @Edad, @EstadoCivil, @Profesion, @Nacionalidad, @Domicilio, @Telefono, @Fax, @Email ) AS t2 
                                           (ExpedienteId, Nombre, Edad, EstadoCivil, Profesion, Nacionalidad, Domicilio, Telefono, Fax, Email)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.Nombre = t2.Nombre,
                                   t1.Edad = t2.Edad,
                                   t1.EstadoCivil = t2.EstadoCivil,
                                   t1.Profesion = t2.Profesion,
                                   t1.Nacionalidad = t2.Nacionalidad,
                                   t1.Domicilio = t2.Domicilio,
                                   t1.Telefono = t2.Telefono,
                                   t1.Fax = t2.Fax,
                                   t1.Email = t2.Email
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, Nombre, Edad, EstadoCivil, Profesion, Nacionalidad, Domicilio, Telefono, Fax, Email) 
                                   values(t2.ExpedienteId, t2.Nombre, t2.Edad, t2.EstadoCivil, t2.Profesion, t2.Nacionalidad, t2.Domicilio, t2.Telefono, t2.Fax, t2.Email);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Nombre = nombre,
                                Edad = edad,
                                EstadoCivil = estadocivil,
                                Profesion = profesion,
                                Nacionalidad = nacionalidad,
                                Domicilio = domicilio,
                                Telefono = tel,
                                Fax = fax,
                                Email = email
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("productor not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("productor {0}:", idsolicitud));

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Productores {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } //  END Productores->productor.dbf

        
        // Resoluciones - resol.dbf
        public static void ImportResoluciones()
        {
            string DBF_FileName = "resol";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromResol = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromResol, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string idresol = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    idresol = dataReader.GetString(1);

                    DateTime fresol = dataReader.GetDateTime(2);
                    DateTime frechazo = dataReader.GetDateTime(3);
                    DateTime fsuspension = dataReader.GetDateTime(4);
                    DateTime fmemorial = dataReader.GetDateTime(5);
                    string articulos = dataReader.GetString(6);
                    string omitio = dataReader.GetString(7);
                    string referente = dataReader.GetString(8);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.DAResoluciones as t1 
                                using(SELECT @ExpedienteId, @Fecha, @Articulos, @Omitio, @Referente ) AS t2 
                                           (ExpedienteId, Fecha, Articulos, Omitio, Referente)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.Fecha = t2.Fecha,
                                   t1.Articulos = t2.Articulos,
                                   t1.Omitio = t2.Omitio,
                                   t1.Referente = t2.Referente
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, Fecha, Articulos, Omitio, Referente) 
                                   values(t2.ExpedienteId, t2.Fecha, t2.Articulos, t2.Omitio, t2.Referente);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Fecha = fresol,
                                Articulos = articulos,
                                Omitio = omitio,
                                Referente = referente
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("resol not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("resol {0}:", idsolicitud));

                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("resol table {0}, {1} tiene errores", idsolicitud, idresol));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();
        } //  END Resoluciones - resol.dbf

        // Solicitantes - solicitante.dbf
        public static void ImportSolicitantes()
        {
            string DBF_FileName = "solicitante";

            string initMain = Utils.getMigracionParam(cnn, "da.main");
            string selectFromSolicitante = string.Format("select * from {0} {1} {2} ORDER BY idsolicitud",
                    DBF_FileName,
                    String.IsNullOrEmpty(initMain) ? "" : "WHERE idsolicitud >=",
                    String.IsNullOrEmpty(initMain) ? "" : "\"" + initMain + "\"");

            OleDbCommand command = new OleDbCommand(selectFromSolicitante, dbaseConn);

            var dataReader = command.ExecuteReader();

            string idsolicitud = string.Empty;
            string nombre = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    idsolicitud = dataReader.GetValue(0).ToString();
                    nombre = dataReader.GetString(1);
                    int edad = int.Parse(dataReader.GetValue(2).ToString());
                    string estadocivil = dataReader.GetString(3);
                    string profesion = dataReader.GetString(4);
                    string domicilio = dataReader.GetString(5);
                    string nacionalidad = dataReader.GetString(6);
                    string lugarnoti = dataReader.GetString(7);
                    string tel = dataReader.GetString(8);
                    string fax = dataReader.GetString(9);
                    string email = dataReader.GetString(10);
                    string calidad = dataReader.GetString(11);
                    string entidadsol = dataReader.GetString(12);
                    string lugarconst = dataReader.GetString(13);
                    string objetosol = dataReader.GetString(14);
                    string encalidad = dataReader.GetString(15);
                    string derechomediante = dataReader.GetString(16);

                    int expedienteId = Utils.getExpedienteIdDeDA(cnn, idsolicitud);

                    if (expedienteId > 0)
                    {
                        cnn.Execute(@"MERGE INTO da.Solicitantes as t1 
                                using(SELECT @ExpedienteId, @Nombre, @Edad, @EstadoCivil, @Profesion, @Domicilio, @Nacionalidad, @LugarNotificacion, @Telefono, @Fax, 
                                            @Email, @Calidad, @EntidadSolicitante, @LugarConstitucion, @ObjetoSolicitud, @EnCalidad, @AdquirioDerecho) AS t2 
                                           (ExpedienteId, Nombre, Edad, EstadoCivil, Profesion, Domicilio, Nacionalidad, LugarNotificacion, Telefono, Fax,
                                            Email, Calidad, EntidadSolicitante, LugarConstitucion, ObjetoSolicitud, EnCalidad, AdquirioDerecho)
                                ON (t1.ExpedienteId = t2.ExpedienteId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                    t1.Nombre = t2.Nombre,
                                    t1.Edad = t2.Edad,
                                    t1.EstadoCivil = t2.EstadoCivil,
                                    t1.Profesion = t2.Profesion,
                                    t1.Domicilio = t2.Domicilio,
                                    t1.Nacionalidad = t2.Nacionalidad,
                                    t1.LugarNotificacion = t2.LugarNotificacion,
                                    t1.Telefono = t2.Telefono,
                                    t1.Fax = t2.Fax,
                                    t1.Email = t2.Email,
                                    t1.Calidad = t2.Calidad,
                                    t1.EntidadSolicitante = t2.EntidadSolicitante,
                                    t1.LugarConstitucion = t2. LugarConstitucion,
                                    t1.ObjetoSolicitud = t2.ObjetoSolicitud,
                                    t1.EnCalidad = t2.EnCalidad,
                                    t1.AdquirioDerecho = t2.AdquirioDerecho
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, Nombre, Edad, EstadoCivil, Profesion, Domicilio, Nacionalidad, LugarNotificacion, Telefono, Fax,
                                            Email, Calidad, EntidadSolicitante, LugarConstitucion, ObjetoSolicitud, EnCalidad, AdquirioDerecho) 
                                   values(t2.ExpedienteId, t2.Nombre, t2.Edad, t2.EstadoCivil, t2.Profesion, t2.Domicilio, t2.Nacionalidad, t2.LugarNotificacion, t2.Telefono, t2.Fax,
                                          t2.Email, t2.Calidad, t2.EntidadSolicitante, t2.LugarConstitucion, t2.ObjetoSolicitud, t2.EnCalidad, t2.AdquirioDerecho);",
                            new
                            {
                                ExpedienteId = expedienteId,
                                Nombre = nombre,
                                Edad = edad,
                                EstadoCivil = estadocivil,
                                Profesion = profesion,
                                Domicilio = domicilio,
                                Nacionalidad = nacionalidad,
                                LugarNotificacion = lugarnoti,
                                Telefono = tel,
                                Fax = fax,
                                Email = email,
                                Calidad = calidad,
                                EntidadSolicitante = entidadsol,
                                LugarConstitucion = lugarconst,
                                ObjetoSolicitud = objetosol,
                                EnCalidad = encalidad,
                                AdquirioDerecho = derechomediante
                            }
                        );
                    }
                    else
                    {
                        log.Error(string.Format("solicitante not found{0}:", idsolicitud));
                    }

                    log.console(string.Format("solicitante {0}:", idsolicitud));
                } //end try
                catch (Exception e)
                {
                    log.Error(string.Format("Solicitantes {0}, {1} tiene errores", idsolicitud, nombre));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();
        } //  END Productores->productor.dbf

    }
}
