using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Timers;
using System.Threading;
using System.Configuration;

namespace DataImport
{
    public class Marcas
    {
        //public const string ConnectionString = @"Data Source=MANOLO\SQLEXPRESS;Initial Catalog=GPI;Integrated Security=False;User ID=express;Password=3xpr3ss;MultipleActiveResultSets=True;";
        public static string ConnectionString;
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

        public class productos
        {
            public string reservas { get; set; }
            public string prods { get; set; }
            public string grafica { get; set; }
        }

        public static productos getProductos(string expediente)
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            var cnn = GetOpenConnection();

            string DBF_FileName = "produc.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");
            conn.Open();

            var products = new productos();
            products.reservas = string.Empty;
            products.prods = string.Empty;
            products.grafica = string.Empty;
            try
            {
                
                using (OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_SOLIC =" + expediente, conn))
                {
                    using (var dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            products.reservas = dataReader.GetString(2);
                            products.prods = dataReader.GetString(3);
                            products.grafica = dataReader.GetString(4);
                            break;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                log.Error(string.Format("produc.dbf {0} has errors:", expediente));
                log.Error(e.Message);

            }
            return products;
        }

        public static void ImportExpedientes()
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            var cnn = GetOpenConnection();

            string DBF_FileName = "MARCA.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string lastNro_Solic = Utils.getMigracionParam(cnn, "marca.nro_solic");
            string selectFromMarca = string.Format("select * from {0} {1} {2}",
                    DBF_FileName,
                    String.IsNullOrEmpty(lastNro_Solic) ? "" : "WHERE nro_solic>=",
                    String.IsNullOrEmpty(lastNro_Solic) ? "" : lastNro_Solic);

            OleDbCommand command = new OleDbCommand(selectFromMarca, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            DateTime timeNow = DateTime.Now;
            string nro_solic = string.Empty;

            while (dataReader.Read())
            {
                
                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    string f_solic = dataReader.GetString(1);
                    string tip_reg = dataReader.GetString(2);
                    string nro_reg = dataReader[3].ToString();
                    string f_status = dataReader.GetString(5);
                    string status = dataReader.GetString(6);
                    string tip_activi = dataReader.GetString(7);
                    string ley = dataReader["Ley"].ToString();
                    string no_time = dataReader["No_time"].ToString();
                    string tip_marca = dataReader["Tip_marca"].ToString();
                    string nom_marca = dataReader["Nom_marca"].ToString();
                    string traducida = dataReader["Traducida"].ToString();
                    bool industrial = dataReader["Activi1"] as int? == 1 ? true : false;
                    bool deServicios = dataReader["Activi2"] as int? == 1 ? true : false;
                    bool comercial = dataReader["Activi3"] as int? == 1 ? true : false;
                    bool certificacion = dataReader["Activi4"] as int? == 1 ? true : false;
                    bool colectiva = dataReader["Activi5"] as int? == 1 ? true : false;
                    bool activi6 = dataReader["Activi6"] as int? == 1 ? true : false;
                    //int? registro = dataReader["nro_Reg"] as int?;
                    //string nro_reg = dataReader["nro_Reg"].ToString();
                    int registro = Utils.parseToInt(nro_reg, DBF_FileName + "(nro_reg)", nro_reg);

                    string raya = dataReader["Raya"].ToString();
                    int? tomo = dataReader["No_tomo"] as int?;
                    int? folio = dataReader["No_folio"] as int?;
                    string doctosAdjuntos = string.Empty;
                    doctosAdjuntos += dataReader["Op1"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op2"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op3"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op4"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op5"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op6"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op7"].ToString() + ",";
                    doctosAdjuntos += dataReader["Op8"].ToString() + ",";
                    string otrosDoctosAdjuntos = dataReader["Otros"].ToString();
                    string ubicacion = dataReader["Ubicacion"].ToString();
                    string ult_renov = dataReader["Ult_renov"].ToString();
                    string ext_marca = dataReader["Ext_marca"].ToString();
                    int? PaisConstituidaId = null;
                    string UbicacionActual = dataReader["Ubicacion"].ToString();
                    string clases = dataReader["Clases"].ToString();

                    int leyId = (ley == "N" ? 1 : 2);

                    DateTime fecha_estatus = Utils.parseDate(f_status, DBF_FileName + "(f_status)", nro_solic);

                    DateTime fecha_solicitud = Utils.parseDate(f_solic, DBF_FileName + "(f_solic)", nro_solic);

                    TimeSpan time = Utils.parseTime(no_time, DBF_FileName + "(no_time)", nro_solic);

                    int found = Utils.getExpedienteIdDeMarcas(cnn, nro_solic);

                    var p = getProductos(nro_solic);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status }).FirstOrDefault();

                    int tipoDeRegistroId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE codigo like @codigo AND ModuloId=1", new { codigo = tip_reg + "%" }).FirstOrDefault();

                    int tipoDeMarca = cnn.Query<int>("select Id FROM dbo.TiposDeMarca WHERE flag like @flag", new { flag = tip_marca + "%" }).FirstOrDefault();

                    int classificacionDeNizaId = cnn.Query<int>("select Id FROM dbo.ClassificacionDeNiza WHERE Codigo like @codigo", new { codigo = clases }).FirstOrDefault();

                    int expedienteId = 0;
                    var xp = new
                    {
                        ModuloId = 1,
                        TipoDeRegistroId = tipoDeRegistroId,
                        Numero = nro_solic,
                        FechaDeSolicitud = fecha_solicitud,
                        Hora = time,
                        EstatusId = statusId,
                        FechaDeEstatus = fecha_estatus,
                        LeyId = leyId,
                        FechaActualizacion = timeNow,
                        expedienteId = found,
                    };

                    if (found > 0)
                    {
                        expedienteId = cnn.Query<int>(@"UPDATE dbo.Expedientes
                            SET TipoDeRegistroId = @TipoDeRegistroId,
                                FechaDeSolicitud = @FechaDeSolicitud,
                                Hora = @Hora,
                                EstatusId = @EstatusId,
                                FechaDeEstatus = @FechaDeEstatus,
                                FechaActualizacion = @FechaActualizacion
                            WHERE Id = @expedienteId;
                            SELECT @expedienteId AS [expedienteId]; ", xp).Single();

                        // UPDATE numero de registro
                        cnn.Execute(@"UPDATE dbo.Marcas 
                                SET Recibo =  @Recibo
                                    ,TipoDeMarca = @TipoDeMarca
                                    ,Denominacion = @Denominacion
                                    ,Traduccion = @Traduccion
                                    ,Industrial = @Industrial
                                    ,DeServicios = @DeServicios
                                    ,Comercial = @Comercial
                                    ,Certificacion = @Certificacion
                                    ,Colectiva = @Colectiva
                                    ,Registro = @Registro
                                    ,Raya = @Raya
                                    ,Tomo = @Tomo
                                    ,Folio = @Folio
                                    ,ClassificacionDeNizaId = @ClassificacionDeNizaId
                                    ,Productos = @Productos
                                    ,Reservas = @Reservas
                                    ,DescripcionGrafica = @DescripcionGrafica
                                    ,DoctosAdjuntos = @DoctosAdjuntos
                                    ,OtrosDoctosAdjuntos = @OtrosDoctosAdjuntos
                                    ,CaracteristicasCom = @CaracteristicasCom
                                    ,EstandaresDeCalidad = @EstandaresDeCalidad
                                    ,AutoridadApReglamento = @AutoridadApReglamento
                                    ,DireccionComercializacion = @DireccionComercializacion
                                    ,UltimaRenovacion = @UltimaRenovacion
                                    ,ExtensionDeLaMarca = @ExtensionDeLaMarca
                                    ,PaisConstituidaId = @PaisConstituidaId
                                    ,UbicacionActual = @UbicacionActual
                                    ,UbicacionAnterior = @UbicacionAnterior
                                    ,FechaDeTraslado = @FechaDeTraslado
                                    ,MotivoDeTraslado = @MotivoDeTraslado
                                WHERE ExpedienteId=@expedienteId",
                            new
                            {                                
                                Recibo = "",
                                TipoDeMarca = tipoDeMarca,
                                Denominacion = nom_marca,
                                Traduccion = traducida,
                                Industrial = industrial,
                                DeServicios = deServicios,
                                Comercial = comercial,
                                Certificacion = certificacion,
                                Colectiva = colectiva,
                                Registro = registro,
                                Raya = raya,
                                Tomo = tomo,
                                Folio = folio,
                                ClassificacionDeNizaId = classificacionDeNizaId,
                                Productos = p.prods,
                                Reservas = p.reservas,
                                DescripcionGrafica = p.grafica,
                                DoctosAdjuntos = doctosAdjuntos,
                                OtrosDoctosAdjuntos = otrosDoctosAdjuntos,
                                CaracteristicasCom = "[pendiente]",
                                EstandaresDeCalidad = "[pendiente]",
                                AutoridadApReglamento = "[pendiente]",
                                DireccionComercializacion = ubicacion,
                                UltimaRenovacion = ult_renov,
                                ExtensionDeLaMarca = ext_marca,
                                PaisConstituidaId = PaisConstituidaId,
                                UbicacionActual = "[pendiente]",
                                UbicacionAnterior = "[pendiente]",
                                FechaDeTraslado = new DateTime(1900, 1, 1),
                                MotivoDeTraslado = "[pendiente]",
                                ExpedienteId = found
                            }
                        );
                    }
                    else {
                        expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId], [FechaActualizacion])
                        VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId, @FechaActualizacion);
                        SELECT SCOPE_IDENTITY() AS [expedienteId]; ", xp ).Single();

                        cnn.Execute(@"INSERT INTO dbo.Marcas([ExpedienteId]
                            ,[Recibo]
                            ,[TipoDeMarca]
                            ,[Denominacion]
                            ,[Traduccion]
                            ,[Industrial]
                            ,[DeServicios]
                            ,[Comercial]
                            ,[Certificacion]
                            ,[Colectiva]
                            ,[Registro]
                            ,[Raya]
                            ,[Tomo]
                            ,[Folio]
                            ,[ClassificacionDeNizaId]
                            ,[Productos]
                            ,[Reservas]
                            ,[DescripcionGrafica]
                            ,[DoctosAdjuntos]
                            ,[OtrosDoctosAdjuntos]
                            ,[CaracteristicasCom]
                            ,[EstandaresDeCalidad]
                            ,[AutoridadApReglamento]
                            ,[DireccionComercializacion]
                            ,[UltimaRenovacion]
                            ,[ExtensionDeLaMarca]
                            ,[PaisConstituidaId]
                            ,[UbicacionActual]
                            ,[UbicacionAnterior]
                            ,[FechaDeTraslado]
                            ,[MotivoDeTraslado])
                        VALUES(@ExpedienteId,
                            @Recibo,
                            @TipoDeMarca,
                            @Denominacion,
                            @Traduccion,
                            @Industrial,
                            @DeServicios,
                            @Comercial,
                            @Certificacion,
                            @Colectiva,
                            @Registro,
                            @Raya,
                            @Tomo,
                            @Folio,
                            @ClassificacionDeNizaId,
                            @Productos,
                            @Reservas,
                            @DescripcionGrafica,
                            @DoctosAdjuntos,
                            @OtrosDoctosAdjuntos,
                            @CaracteristicasCom,
                            @EstandaresDeCalidad,
                            @AutoridadApReglamento,
                            @DireccionComercializacion,
                            @UltimaRenovacion,
                            @ExtensionDeLaMarca,
                            @PaisConstituidaId,
                            @UbicacionActual,
                            @UbicacionAnterior,
                            @FechaDeTraslado,
                            @MotivoDeTraslado
                        )",
                         new
                         {
                             ExpedienteId = expedienteId,
                             Recibo = "",
                             TipoDeMarca = tipoDeMarca,
                             Denominacion = nom_marca,
                             Traduccion = traducida,
                             Industrial = industrial,
                             DeServicios = deServicios,
                             Comercial = comercial,
                             Certificacion = certificacion,
                             Colectiva = colectiva,
                             Registro = registro,
                             Raya = raya,
                             Tomo = tomo,
                             Folio = folio,
                             ClassificacionDeNizaId = classificacionDeNizaId,
                             Productos = p.prods,
                             Reservas = p.reservas,
                             DescripcionGrafica = p.grafica,
                             DoctosAdjuntos = doctosAdjuntos,
                             OtrosDoctosAdjuntos = otrosDoctosAdjuntos,
                             CaracteristicasCom = "[pendiente]",
                             EstandaresDeCalidad = "[pendiente]",
                             AutoridadApReglamento = "[pendiente]",
                             DireccionComercializacion = ubicacion,
                             UltimaRenovacion = ult_renov,
                             ExtensionDeLaMarca = ext_marca,
                             PaisConstituidaId = PaisConstituidaId,
                             UbicacionActual = "[pendiente]",
                             UbicacionAnterior = "[pendiente]",
                             FechaDeTraslado = new DateTime(1900, 1, 1),
                             MotivoDeTraslado = "[pendiente]"
                         }
                        );
                    }

                    log.console(string.Format("Expediente {0} importado:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0} has errors:", nro_solic));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        public static void ImportCrono()
        {
            var cnn = GetOpenConnection();

            bool sample = false;
            DateTime todayMinus5 = Utils.getInitDateForCrono();
            string ultimaFecha = todayMinus5.Year.ToString() + todayMinus5.Month.ToString().PadLeft(2, '0') + todayMinus5.Day.ToString().PadLeft(2, '0');

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string allCrono = Utils.getMigracionParam(cnn, "marcas.crono");

            string DBF_FileName = "evento.dbf";
            
            string selectFromEvento = String.Empty;
            if (allCrono.Equals("1")) //all crono
            {
                selectFromEvento = string.Format("select * from {0}", DBF_FileName);
            }
            else { //last few days
                selectFromEvento = string.Format("select * from {0} {1} {2}",
                                        DBF_FileName,
                                        "WHERE f_evento>=",
                                        sample ? "\"20140101\"" : "\"" + ultimaFecha + "\"");
            }

            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            OleDbCommand command = new OleDbCommand(selectFromEvento, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            string nro_solic = string.Empty;

            while (dataReader.Read())
            {

                int usuarioId = 0;
                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    string f_evento = dataReader.GetString(1);
                    string status = dataReader.GetString(3);
                    string referencia = dataReader.GetString(4);
                    string persona = dataReader.GetString(5);
                    string observaciones = dataReader.GetString(7);
                    
                    int expedienteId = Utils.getExpedienteIdDeMarcas(cnn, nro_solic);

                    DateTime fecha = Utils.parseDate(f_evento, DBF_FileName + "(f_evento)", nro_solic);

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status }).FirstOrDefault();

                    int found = cnn.Query<int>("SELECT id FROM Cronologia WHERE ExpedienteId = @ExpedienteId AND Fecha = @Fecha AND EstatusId = @EstatusId", 
                            new {
                                    ExpedienteId = expedienteId,
                                    Fecha = fecha,
                                    EstatusId = statusId
                            }).FirstOrDefault();

                    if (found == 0) //if evento cronologico not found
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
                                 UsuarioId = usuarioId,
                                 Observaciones = observaciones,
                                 UsuarioIniciales = persona
                             }
                            );
                    }
                    else
                    {
                        log.Error(nro_solic);

                        // expedientes inexistentes 
                        //198500055
                        //199900000
                        //199900000
                        //197000001
                        //199500000
                    }
                    log.console(string.Format("Crono {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Cronologia del expediente Expediente {0} tiene errores:", nro_solic));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            //updating migration log
            //Utils.updateMigracionLog(cnn, "evento", recordCounter);

            conn.Close();  //close connection to the .dbf file
            return;
        }


        //
        //DELETE FOR EMPTY(FECHA) AND EMPTY(PAIS) AND EMPTY(NUMERO)
        //PACK
        public static void ImportPrioridades()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string initPriorida = Utils.getMigracionParam(cnn, "marcas.priorida");

            string DBF_FileName = "priorida.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string selectFromPriorida = string.Empty;
            if (initPriorida.Equals("0")) //all
            {
                selectFromPriorida = string.Format("select * from {0}", DBF_FileName);
            }
            else
            {
                selectFromPriorida = string.Format("select * from {0} WHERE nro_solic >= {1}00000",
                                        DBF_FileName,
                                        initPriorida);
            }

            OleDbCommand command = new OleDbCommand(selectFromPriorida, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            string nro_solic = string.Empty;
            string numero = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    numero = dataReader.GetString(3);

                    DateTime f_prioridad = dataReader.GetDateTime(1);

                    DateTime? fecha = Utils.DateCheck(f_prioridad, DBF_FileName + "(f_prioridad)", nro_solic);

                    string pais = dataReader.GetString(2);
                    
                    int expedienteId = Utils.getExpedienteIdDeMarcas(cnn, nro_solic);
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, prioridad numero: {1}", nro_solic, numero));
                        continue;
                    }
                    if (fecha == null)
                    {
                        log.Error(string.Format("Invalid Fecha Expediente {0} no encontrado, prioridad numero: {1}", nro_solic, numero));
                        continue;
                    }

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();

                    int found = cnn.Query<int>("SELECT Id FROM [dbo].[PrioridadMarcas] WHERE ExpedienteId = @ExpedienteId AND Numero = @Numero", 
                            new { ExpedienteId = expedienteId, Numero = numero }).FirstOrDefault();

                    if (found == 0)
                    {
                        cnn.Execute(@"INSERT INTO dbo.PrioridadMarcas([ExpedienteId]
                            ,[Fecha]
                            ,[PaisId]
                            ,[Numero])
                        VALUES(@ExpedienteId,
                            @Fecha,
                            @PaisId,
                            @Numero
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 Fecha = fecha,
                                 PaisId = paisId,
                                 Numero = string.IsNullOrEmpty(numero) ? string.Empty : numero.Trim()
                             }
                        );
                    }
                    log.console(string.Format("Prioridad {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Prioridad del Expediente: {0} tiene errores numero:{1}", nro_solic, numero));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        public class Titular
        {
            public string nombre { get; set; }
        }

        private static Titular getTitular(string nro_tit)
        {
            string FilePath = ConfigurationManager.AppSettings["legacyDb"];

            string DBF_FileName = "titular.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");
            conn.Open();

            var titular = new Titular();
            using (OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE nro_tit =" + nro_tit, conn))
            {
                using (var dataReader2 = command.ExecuteReader())
                {
                    while (dataReader2.Read())
                    {
                        titular.nombre = dataReader2.GetString(1);
                        break;
                    }

                }
            }
            return titular;
        }



        //
        //PACK
        public static void ImportTITULARES()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            
            string DBF_FileName = "titumar.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string initTit = Utils.getMigracionParam(cnn, "marcas.tit");
            string selectFromTitumar = string.Empty;
            if (initTit.Equals("0")) //all
            {
                selectFromTitumar = string.Format("select * from {0}", DBF_FileName);
            }
            else
            {
                selectFromTitumar = string.Format("select * from {0} WHERE nro_solic >= {1}00000",
                                        DBF_FileName,
                                        initTit);
            }
            log.Error(selectFromTitumar);

            OleDbCommand command = new OleDbCommand(selectFromTitumar, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            string nro_solic = string.Empty;
            string nro_tit = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    nro_tit = dataReader.GetValue(1).ToString();

                    string dir_noti = dataReader.GetString(2);
                    string dir_ubi = dataReader.GetString(3);
                    string pais = dataReader.GetString(4);

                    var titular = getTitular(nro_tit);
                    
                    int expedienteId = Utils.getExpedienteIdDeMarcas(cnn, nro_solic);
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, titular numero: {1}", nro_solic, nro_tit));
                        continue;
                    }

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pais }).FirstOrDefault();

                    var titularId = InsertTitular(cnn, titular.nombre, dir_ubi, paisId, int.Parse(nro_tit));

                    int found = cnn.Query<int>("SELECT Id FROM [dbo].[TitularesDeLaMarca] WHERE ExpedienteId = @ExpedienteId AND TitularId = @TitularId",
                            new { ExpedienteId = expedienteId, TitularId = titularId }).FirstOrDefault();

                    if (titularId > 0 && found == 0 )
                    {
                        cnn.Execute(@"INSERT INTO dbo.[TitularesDeLaMarca]([ExpedienteId]
                            ,[TitularId]
                            ,[DireccionParaNotificacion]
                            ,[DireccionParaUbicacion]
                            ,[EnCalidadDe]
                            )
                        VALUES(@ExpedienteId,
                            @TitularId,
                            @DireccionParaNotificacion,
                            @DireccionParaUbicacion,
                            @EnCalidadDe
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 TitularId = titularId,
                                 DireccionParaNotificacion = string.IsNullOrEmpty(dir_noti) ? string.Empty : dir_noti.Trim(),
                                 DireccionParaUbicacion = string.IsNullOrEmpty(dir_ubi) ? string.Empty : dir_ubi.Trim(),
                                 EnCalidadDe = ""
                             }
                            );
                    }
                    log.console(string.Format("TitularesDeLaMarca {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("titumar nro_solic:{0} tiene errores titular numero:{1}", nro_solic, nro_tit));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        private static int InsertTitular(SqlConnection cnn, string nombre, string dir, int paisId, int oldId)
        {
            int titularId = 0;
            nombre = String.IsNullOrEmpty(nombre) ? string.Empty : nombre.Trim();
            dir = String.IsNullOrEmpty(dir) ? string.Empty : dir.Trim();
            oldId = oldId == 0 ? Utils.GetRandomNumber(1, 100000) : oldId;

            try
            {
                titularId = cnn.Query<int>("SELECT Id FROM [dbo].[Titulares] WHERE Nombre = @nombre AND Direccion = @direccion", new { nombre = nombre, direccion = dir }).FirstOrDefault();

                if (titularId == 0) { 
                    //si el titular ya existe no lo agregamos
                    titularId = cnn.Query<int>(@"INSERT INTO dbo.[Titulares]([Nombre]
                                  ,[Direccion]
                                  ,[PaisId]
                                  ,[OldId]
                                )
                            SELECT @Nombre, @Direccion, @PaisId, @OldId
                            WHERE NOT EXISTS(
                                SELECT 1
                                FROM dbo.[Titulares]
                                WHERE OldId = @OldId
                            );
                            SELECT Id AS [Id]
                            FROM dbo.[Titulares]
                            WHERE OldId = @OldId;",
                         new
                         {
                             Nombre = nombre,
                             Direccion = dir,
                             PaisId = paisId,
                             OldId = oldId
                         }
                        ).Single();
                }
            }
            catch (Exception e)
            {
                log.Error(string.Format("Error en titular titular numero:{0}", oldId));
                log.Error(e.Message);
            }

            return titularId;
        }



        //
        // MANDATARIOS
        //
        public static void ImportMANDATARIOS()
        {            
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            
            string DBF_FileName = "MANDATAR.DBF";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string initMandatar = Utils.getMigracionParam(cnn, "marcas.mandatar");
            string selectFromMandatar = string.Empty;

            if (initMandatar.Equals("0")) //all crono
            {
                selectFromMandatar = string.Format("select * from {0}", DBF_FileName);
            }
            else
            {
                selectFromMandatar = string.Format("select * from {0} WHERE nro_mandat >= {1}00000",
                                        DBF_FileName,
                                        initMandatar);
            }
            log.Error(selectFromMandatar);

            //OleDbCommand command = new OleDbCommand("select * from " + DBF_FileName + " WHERE NRO_MANDAT  >201410564", conn);
            OleDbCommand command = new OleDbCommand(selectFromMandatar, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();

            string nro_solic = string.Empty;
            string nom_mandat = string.Empty;

            while (dataReader.Read())
            {
                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    nom_mandat = dataReader.GetString(1);

                    int expedienteId = Utils.getExpedienteIdDeMarcas(cnn, nro_solic);
                    if (expedienteId == 0)
                    {
                        log.Error(string.Format("Expediente {0} no encontrado, mandatario: {1}", nro_solic, nom_mandat));
                        continue;
                    }

                    var mandatarioId = InsertMandatario(cnn, nom_mandat);
                    int found = cnn.Query<int>("SELECT Id FROM [dbo].[MandatarioDeLaMarca] WHERE ExpedienteId = @ExpedienteId AND MandatarioId = @MandatarioId",
                        new { ExpedienteId = expedienteId, MandatarioId = mandatarioId }).FirstOrDefault();

                    if (mandatarioId > 0 && found == 0)
                    {

                        cnn.Execute(@"INSERT INTO dbo.[MandatarioDeLaMarca]([ExpedienteId]
                            ,[MandatarioId]
                            )
                        VALUES(@ExpedienteId,
                            @MandatarioId
                        )",
                             new
                             {
                                 ExpedienteId = expedienteId,
                                 MandatarioId = mandatarioId
                             }
                            );
                    }

                    log.console(string.Format("MandatarioDeLaMarca {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente {0} con errores: {1}", nro_solic, nom_mandat));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        private static int InsertMandatario(SqlConnection cnn, string nombre)
        {
            int Id = 0;
            try
            {
                Id = cnn.Query<int>(@"INSERT INTO dbo.[Mandatarios]([Nombre])
                        SELECT @Nombre
                        WHERE NOT EXISTS(
                            SELECT 1
                            FROM dbo.[Mandatarios]
                            WHERE Nombre = @Nombre
                        );
                        SELECT Id AS [Id]
                        FROM dbo.[Mandatarios]
                        WHERE Nombre = @Nombre;",
                     new
                     {
                         Nombre = string.IsNullOrEmpty(nombre)? string.Empty: nombre.Trim()
                     }
                    ).Single();
            }
            catch (Exception e)
            {
                log.Error(string.Format("Expediente tiene errores, mandatario:{0}", nombre));
                log.Error(e.Message);
            }

            return Id;
        }



        //
        // DELETE FOR EMPTY(f_solic) AND EMPTY(nro_solic)
        // DELETE FOR EMPTY(tip_anota)

        /*
          DELETE FROM anota WHERE EMPTY(tip_anota)
          DELETE from ANOTA WHERE tip_anota='RE'
          PACK
         */
        public static void ImportAnotaciones()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "anota.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string initAnota = Utils.getMigracionParam(cnn, "marcas.anota");
            string selectFromAnota = string.Empty;

            if (initAnota.Equals("0")) //all crono
            {
                selectFromAnota = string.Format("select * from {0}", DBF_FileName);
            }
            else
            {
                selectFromAnota = string.Format("select * from {0} WHERE nro_solic >= {1}0000",
                                        DBF_FileName,
                                        initAnota);
            }
            log.Error(selectFromAnota);

            OleDbCommand command = new OleDbCommand(selectFromAnota, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            DateTime timeNow = DateTime.Now;

            string nro_solic = string.Empty;
            string tip_anota = string.Empty;
            string f_solic = string.Empty;
            string status = string.Empty;
            string nom_tit = string.Empty;
            string pai_tit = string.Empty;
            string f_status = string.Empty;
            string ley = string.Empty;
            string nro_tit = string.Empty;
            string nro_ag = string.Empty;
            string dir_tit = string.Empty;

            while (dataReader.Read())
            {

                TimeSpan time = new TimeSpan(0, 0, 0);

                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    tip_anota = dataReader.GetValue(1).ToString();
                    f_solic = dataReader.GetString(2);
                    status = dataReader.GetValue(3).ToString();
                    nom_tit = dataReader.GetValue(4).ToString();
                    pai_tit = dataReader.GetValue(5).ToString();
                    f_status = dataReader.GetString(6);
                    ley = dataReader.GetValue(8).ToString();
                    nro_tit = dataReader.GetValue(9).ToString();
                    nro_ag = dataReader.GetValue(10).ToString();
                    dir_tit = dataReader.GetValue(11).ToString();

                    if (tip_anota == "RE" || tip_anota == "")
                    {
                        continue;
                    }

                    int tipoDeRegistroId = 35;
                    string status_code = tip_anota + '-' + status;

                    int leyId = 1;
                    if (ley.Equals("N"))
                        leyId = 2;

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status_code }).FirstOrDefault();

                    int paisId = cnn.Query<int>("SELECT Id FROM [dbo].[paises] WHERE codigo = @codigo", new { codigo = pai_tit }).FirstOrDefault();

                    DateTime fecha_solicitud = Utils.parseDate(f_solic, DBF_FileName + "(f_solic)", nro_solic);
                    DateTime fecha_estatus = Utils.parseDate(f_status, DBF_FileName + "(f_status)", nro_solic);

                    int expedienteId = Utils.getExpedienteIdDeAnotaciones(cnn, tipoDeRegistroId, nro_solic);
                    
                    if (expedienteId == 0) {
                        expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId], [FechaActualizacion])
                            VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId, @FechaActualizacion);
                            SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                                new
                                {
                                    ModuloId = 4,
                                    TipoDeRegistroId = tipoDeRegistroId,
                                    Numero = nro_solic,
                                    FechaDeSolicitud = fecha_solicitud,
                                    Hora = time,
                                    EstatusId = statusId,
                                    FechaDeEstatus = fecha_estatus,
                                    LeyId = leyId,
                                    FechaActualizacion = timeNow
                                }
                            ).Single();
                    }
                    else
                    {
                        expedienteId = cnn.Query<int>(@"UPDATE dbo.Expedientes
                        SET FechaDeSolicitud = @FechaDeSolicitud,
                            Hora = @Hora,
                            EstatusId = @EstatusId,
                            FechaDeEstatus = @FechaDeEstatus,
                            FechaActualizacion = @FechaActualizacion
                        WHERE Id = @expedienteId;
                        SELECT @expedienteId AS [expedienteId]; ",
                            new
                            {
                                FechaDeSolicitud = fecha_solicitud,
                                Hora = time,
                                EstatusId = statusId,
                                FechaDeEstatus = fecha_estatus,
                                expedienteId = expedienteId,
                                FechaActualizacion = timeNow
                            }
                            ).Single();
                    }
                    
                    int titularId = InsertTitular(cnn, nom_tit, dir_tit, paisId, int.Parse(nro_tit));

                    int anotacionesId = cnn.Query<int>(@"MERGE INTO dbo.Anotaciones as t1
                                using(SELECT @ExpedienteId, @TitularId, @NuevoTitular, @Direccion, @PaisId) AS t2 (ExpedienteId, TitularId, NuevoTitular, Direccion, PaisId)
                                ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TitularId = t2.TitularId)
                                WHEN MATCHED THEN
                                UPDATE SET
                                   t1.NuevoTitular = t2.NuevoTitular, 
                                   t1.Direccion = t2.Direccion,
                                   t1.PaisId = t2.PaisId
                                WHEN NOT MATCHED THEN
                                   insert (ExpedienteId, TitularId, NuevoTitular, Direccion, PaisId) values(t2.ExpedienteId, t2.TitularId, t2.NuevoTitular, t2.Direccion, t2.PaisId);
                        SELECT 0 AS [expedienteId];",
                                new
                                {
                                    ExpedienteId = expedienteId,
                                    TitularId = titularId,
                                    NuevoTitular = nom_tit,
                                    Direccion = dir_tit,
                                    PaisId = paisId
                                }
                        ).Single();

                    log.console(string.Format("anota {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("(ImportAnotaciones) Expediente tiene errores :{0}", nro_solic));
                    log.Error(string.Format("(ImportAnotaciones) {0}, {1}, {2}, {3}, {4}, {5}", tip_anota, nro_solic, f_solic, time, status, f_status, ley));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        public static void ImportAnotacionRegistros()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "anotreg.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string initAnotReg = Utils.getMigracionParam(cnn, "marcas.anotreg");
            string selectFromAnotReg = string.Empty;

            if (initAnotReg.Equals("0")) //all crono
            {
                selectFromAnotReg = string.Format(@"SELECT ar.*, a.tip_anota as tipo, a.status 
                                                    FROM {0} ar 
                                                    INNER JOIN anota.dbf a ON ar.nro_solic = a.nro_solic 
                                                    WHERE ar.nro_solic in (SELECT nro_solic from anota GROUP BY nro_solic HAVING count(*)>=1)", DBF_FileName);
            }
            else
            {
                selectFromAnotReg = string.Format(@"SELECT ar.*, a.tip_anota as tipo, a.status 
                                                FROM {0} ar 
                                                INNER JOIN anota.dbf a ON ar.nro_solic = a.nro_solic 
                                                WHERE ar.nro_solic in (SELECT nro_solic from anota WHERE nro_solic > {1}0000  GROUP BY nro_solic HAVING count(*)>=1)",
                                        DBF_FileName,
                                        initAnotReg);
            }
            log.Error(selectFromAnotReg);

            //OleDbCommand command = new OleDbCommand("SELECT ar.*, a.tip_anota as tipo, a.status FROM " + DBF_FileName + " ar INNER JOIN anota.dbf a ON ar.nro_solic = a.nro_solic WHERE ar.nro_solic in (SELECT nro_solic from anota WHERE nro_solic > 20120000 GROUP BY nro_solic HAVING count(*)=1)", conn);
            OleDbCommand command = new OleDbCommand(selectFromAnotReg, conn);

            conn.Open();
            var dataReader = command.ExecuteReader();
            int counter = 0;
            DateTime timeNow = DateTime.Now;

            string nro_solic = string.Empty;
            string nro_reg = string.Empty;
            string tip_reg = string.Empty;
            string raya = string.Empty;
            string tipoAnotacion = string.Empty;

            while (dataReader.Read())
            {
                log.console(string.Format("record: {0}", ++counter));

                try
                {
                    nro_solic = dataReader.GetValue(0).ToString();
                    nro_reg = dataReader.GetValue(1).ToString();
                    tip_reg = dataReader.GetString(2);
                    raya = dataReader.GetString(3);
                    tipoAnotacion = dataReader.GetString(5);

                    string status = dataReader.GetValue(6).ToString();

                    if (tipoAnotacion == "RE" || tipoAnotacion == "") //renovaciones in renova.dbf
                    {
                        continue;
                    }

                    int tipoDeRegistroId = 35;

                    int tipoDeAnotacionId = 0;

                    if (tipoAnotacion.Equals("CN"))
                    {
                        tipoDeAnotacionId = 31;
                    }
                    else if (tipoAnotacion.Equals("CA"))
                    {
                        tipoDeAnotacionId = 28;
                    }
                    else if (tipoAnotacion.Equals("LI"))
                    {
                        tipoDeAnotacionId = 32;
                    }
                    else if (tipoAnotacion.Equals("TR"))
                    {
                        tipoDeAnotacionId = 34;
                    }

                    string status_code = tipoAnotacion + '-' + status;

                    int expedienteAnotacion = Utils.getExpedienteIdDeAnotaciones(cnn, tipoDeRegistroId, nro_solic);

                    if (expedienteAnotacion == 0)
                    {
                        log.Error(string.Format("Expediente de anotaciones no encontrado, anotreg: {0} {1} {2}", nro_solic, tipoAnotacion, tipoDeRegistroId));
                        continue;
                    }

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status_code }).FirstOrDefault();
                    if (statusId == 0)
                    {
                        log.Error(string.Format("Estatus en anotreg invalido: {0} {1}", nro_solic, tipoAnotacion));
                        continue;
                    }

                    int tipoDeRegistroDeMarcasId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE codigo like @codigo AND ModuloId=1", new { codigo = tip_reg + "%" }).FirstOrDefault();
                    if (tipoDeRegistroDeMarcasId == 0)
                    {
                        log.Error(string.Format("Expediente invalido, anotreg: {0}, {1}, {2} {3}", nro_solic, nro_reg, raya, tip_reg));
                        continue;
                    }

                    int expedienteDeMarcasId = cnn.Query<int>(@"SELECT ExpedienteId 
                                                                FROM dbo.Marcas m
                                                                INNER JOIN expedientes e ON e.Id = m.ExpedienteId
                                                                WHERE registro = @registro AND raya=@raya AND e.TipoDeRegistroId = @tipoDeRegistroDeMarcasId", 
                                                                new { registro = nro_reg, raya = raya.Trim(), tipoDeRegistroDeMarcasId = tipoDeRegistroDeMarcasId }).FirstOrDefault();
                    if (expedienteDeMarcasId == 0)
                    {
                        log.Error(string.Format("Expediente no encontrado, anotreg: {0}, {1}, {2}", nro_solic, nro_reg, raya));
                        continue;
                    }

                    cnn.Execute(@"MERGE INTO dbo.AnotacionEnExpedientes as t1 
                        using(SELECT @ExpedienteId, @TipoDeAnotacionId, @EnExpedienteId, @EnRegistro, @Raya,  @EstatusId, @TipoDeRegistroId) AS t2 
                                     (ExpedienteId, TipoDeAnotacionId, EnExpedienteId, EnRegistro, Raya, EstatusId, TipoDeRegistroId )
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TipoDeAnotacionId = t2.TipoDeAnotacionId AND t1.EnRegistro = t2.EnRegistro AND t1.Raya = t2.Raya AND t1.TipoDeRegistroId = t2.TipoDeRegistroId)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.EstatusId = t2.EstatusId, t1.FechaActualizacion = @FechaActualizacion
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, TipoDeAnotacionId, EnRegistro, Raya, EnExpedienteId, EstatusId, FechaActualizacion, TipoDeRegistroId) 
                           values(t2.ExpedienteId, t2.TipoDeAnotacionId, t2.EnRegistro, t2.Raya, t2.EnExpedienteId, t2.EstatusId, @FechaActualizacion, t2.TipoDeRegistroId);",
                        new
                        {
                            ExpedienteId = expedienteAnotacion,
                            TipoDeAnotacionId = tipoDeAnotacionId,
                            EnRegistro = nro_reg,
                            Raya = raya,
                            EnExpedienteId = expedienteDeMarcasId,
                            EstatusId = statusId,
                            FechaActualizacion = timeNow,
                            TipoDeRegistroId = tipoDeRegistroDeMarcasId
                        }
                    );
                    log.console(string.Format("anotreg {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("Expediente en anotreg {0}, {1} tiene errores", nro_solic, tipoAnotacion));
                    log.Error(e.Message);
                }

            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }


        public static void ImportRenovaciones()
        {
            var cnn = GetOpenConnection();

            string FilePath = ConfigurationManager.AppSettings["legacyDb"];
            string DBF_FileName = "renova.dbf";
            OleDbConnection conn = new OleDbConnection("Provider=vfpoledb;Data Source=" + FilePath + ";Extended Properties=dBASE IV;");

            string initRenova = Utils.getMigracionParam(cnn, "marcas.renova");
            string selectFromAnota = string.Empty;

            if (initRenova.Equals("0")) //all crono
            {
                selectFromAnota = string.Format("select * from {0}", DBF_FileName);
            }
            else
            {
                selectFromAnota = string.Format("select * from {0} WHERE nro_renov >= {1}0000",
                                        DBF_FileName,
                                        initRenova);
            }

            log.Error(selectFromAnota);

            OleDbCommand command = new OleDbCommand(selectFromAnota, conn);
            conn.Open();
            var dataReader = command.ExecuteReader();
            DateTime timeNow = DateTime.Now;

            string nro_reg = string.Empty;
            string nro_solic = string.Empty;
            string f_solic = string.Empty;
            string tip_reg = string.Empty;
            string raya = string.Empty;
            string status = string.Empty;
            string f_status = string.Empty;
            string nro_ag = string.Empty;
            string ley = string.Empty;

            while (dataReader.Read())
            {

                try
                {
                    nro_reg = dataReader.GetValue(0).ToString();
                    nro_solic = dataReader.GetValue(1).ToString();
                    f_solic = dataReader.GetString(2);
                    tip_reg = dataReader.GetString(3);
                    raya = dataReader["Raya"].ToString();
                    status = dataReader.GetValue(5).ToString();
                    f_status = dataReader.GetString(6);
                    nro_ag = dataReader.GetValue(7).ToString();
                    ley = dataReader.GetValue(8).ToString();

                    string tip_anota = "RE";
                    TimeSpan time = new TimeSpan(0, 0, 0);

                    int tipoDeRegistroId = 33; //35 only for CA,CN,LI,TR
                    int tipoDeAnotacionId = 33; //RE (Renovacion)

                    string status_code = tip_anota + '-' + status;

                    int leyId = 1;
                    if (ley.Equals("N"))
                        leyId = 2;

                    int statusId = cnn.Query<int>("select Id FROM dbo.Estatus WHERE Codigo = @codigo", new { codigo = status_code }).FirstOrDefault();

                    DateTime fecha_solicitud = Utils.parseDate(f_solic, DBF_FileName + "(f_solic)", nro_solic);
                    DateTime fecha_estatus = Utils.parseDate(f_status, DBF_FileName + "(f_status)", nro_solic);

                    int expedienteId = Utils.getExpedienteIdDeRenovaciones(cnn, tipoDeRegistroId, nro_solic);

                    int tipoDeRegistroDeMarcasId = cnn.Query<int>("select Id FROM dbo.TiposDeRegistro WHERE codigo like @codigo AND ModuloId=1", new { codigo = tip_reg + "%" }).FirstOrDefault();
                    if (tipoDeRegistroDeMarcasId == 0)
                    {
                        log.Error(string.Format("Expediente invalido, anotreg: {0}, {1}, {2} {3}", nro_solic, nro_reg, raya, tip_reg));
                        continue;
                    }

                    int expedienteDeMarcasId = cnn.Query<int>(@"SELECT ExpedienteId
                                                                FROM dbo.Marcas m
                                                                INNER JOIN expedientes e ON e.Id = m.ExpedienteId
                                                                WHERE registro = @registro AND raya=@raya AND e.TipoDeRegistroId = @tipoDeRegistroDeMarcasId",
                                                                new { registro = nro_reg, raya = raya.Trim(), tipoDeRegistroDeMarcasId = tipoDeRegistroDeMarcasId }).FirstOrDefault();
                    if (expedienteDeMarcasId == 0)
                    {
                        log.Error(string.Format("Expediente no encontrado, anotreg: {0}, {1}, {2}", nro_solic, nro_reg, raya));
                        continue;
                    }

                    if (expedienteId == 0)
                    {
                        expedienteId = cnn.Query<int>(@"INSERT INTO dbo.Expedientes([ModuloId],[TipoDeRegistroId],[Numero],[FechaDeSolicitud],[Hora],[EstatusId],[FechaDeEstatus],[LeyId], [FechaActualizacion])
                            VALUES( @ModuloId, @TipoDeRegistroId, @Numero, @FechaDeSolicitud, @Hora, @EstatusId, @FechaDeEstatus, @LeyId, @FechaActualizacion);
                            SELECT SCOPE_IDENTITY() AS [expedienteId]; ",
                                new
                                {
                                    ModuloId = 5,
                                    TipoDeRegistroId = tipoDeRegistroId,
                                    Numero = nro_solic,
                                    FechaDeSolicitud = fecha_solicitud,
                                    Hora = time,
                                    EstatusId = statusId,
                                    FechaDeEstatus = fecha_estatus,
                                    LeyId = leyId,
                                    FechaActualizacion = timeNow
                                }
                            ).Single();
                    }
                    else {
                        expedienteId = cnn.Query<int>(@"UPDATE dbo.Expedientes
                        SET TipoDeRegistroId = @TipoDeRegistroId,
                            FechaDeSolicitud = @FechaDeSolicitud,
                            Hora = @Hora,
                            EstatusId = @EstatusId,
                            FechaDeEstatus = @FechaDeEstatus,
                            FechaActualizacion = @FechaActualizacion
                        WHERE Id = @expedienteId;
                        SELECT @expedienteId AS [expedienteId]; ",
                            new
                            {
                                TipoDeRegistroId = tipoDeRegistroId,
                                FechaDeSolicitud = fecha_solicitud,
                                Hora = time,
                                EstatusId = statusId,
                                FechaDeEstatus = fecha_estatus,
                                expedienteId = expedienteId,
                                FechaActualizacion = timeNow
                            }
                            ).Single();
                    }

                    // update agente

                    cnn.Execute(@"MERGE INTO dbo.AnotacionEnExpedientes as t1 
                        using(SELECT @ExpedienteId, @TipoDeAnotacionId, @EnRegistro, @Raya, @EnExpedienteId, @EstatusId, @TipoDeRegistroId) AS t2 
                                     (ExpedienteId, TipoDeAnotacionId, EnRegistro, Raya, EnExpedienteId, EstatusId, TipoDeRegistroId )
                        ON (t1.ExpedienteId = t2.ExpedienteId AND t1.TipoDeAnotacionId = t2.TipoDeAnotacionId AND t1.EnExpedienteId = t2.EnExpedienteId)
                        WHEN MATCHED THEN
                        UPDATE SET
                           t1.EstatusId = t2.EstatusId,
                           t1.FechaActualizacion = @FechaActualizacion
                        WHEN NOT MATCHED THEN
                           insert (ExpedienteId, TipoDeAnotacionId, EnRegistro, Raya, EnExpedienteId, EstatusId, FechaActualizacion, TipoDeRegistroId) 
                           values(t2.ExpedienteId, t2.TipoDeAnotacionId, t2.EnRegistro, t2.Raya, t2.EnExpedienteId, t2.EstatusId, @FechaActualizacion, t2.TipoDeRegistroId);",
                        new
                        {
                            ExpedienteId = expedienteId,
                            TipoDeAnotacionId = tipoDeAnotacionId,
                            EnRegistro = nro_reg,
                            Raya = raya,
                            EnExpedienteId = expedienteDeMarcasId,
                            EstatusId = statusId,
                            FechaActualizacion = timeNow,
                            TipoDeRegistroId = tipoDeRegistroDeMarcasId
                        }
                    );

                    log.console(string.Format("renova {0}:", nro_solic));
                }
                catch (Exception e)
                {
                    log.Error(string.Format("(renova) Expediente tiene errores :{0}", nro_solic));
                    log.Error(string.Format("(renova) {0}, {1}, {2}, {3}, {4}", nro_solic, f_solic, status, f_status, ley));
                    log.Error(e.Message);
                }
            }
            dataReader.Close();

            conn.Close();  //close connection to the .dbf file
            return;
        }

        public static void CreateCronoAnotacionesPublicadas()
        {
            var cnn = GetOpenConnection();
            cnn.Execute(@"MERGE INTO dbo.CRONOLOGIA AS t1 
                            USING (
	                            SELECT e.Id as ExpedienteId, FechaPublicacion, 224, 'Gaceta', 0, 'edicto-publicado', '_system', null, null, cr.Id as cronoId
	                                                        FROM gaceta g
	                                                        LEFT JOIN Expedientes e ON g.Numero = e.Numero and g.TipoDeRegistroId = e.TipoDeRegistroId and ModuloId=4
	                                                        LEFT JOIN Cronologia cr ON cr.ExpedienteId = e.Id AND cr.EstatusId=224
	                                                        WHERE g.TipoDeRegistroId=35
                            ) AS t2 
                            (ExpedienteId,Fecha,EstatusId,Referencia,UsuarioId,Observaciones,UsuarioIniciales,JSONDOC,HTMLDOC,cronoId)
                            ON (t1.ExpedienteId = t2.ExpedienteId AND t1.Id = t2.cronoId)
                            WHEN MATCHED THEN
                            UPDATE SET
	                            t1.Fecha = t2.Fecha, t1.JSONDOC = t2.JSONDOC, t1.HTMLDOC = t2.HTMLDOC
                            WHEN NOT MATCHED THEN
                            INSERT (ExpedienteId,Fecha,EstatusId,Referencia,UsuarioId,Observaciones,UsuarioIniciales,JSONDOC,HTMLDOC) 
                            VALUES(t2.ExpedienteId, t2.Fecha, t2.EstatusId, t2.Referencia, t2.UsuarioId, t2.Observaciones, t2.UsuarioIniciales, t2.JSONDOC, t2.HTMLDOC);");
        }


        public static void CreateCronoRenoPublicadas()
        {
            var cnn = GetOpenConnection();
            cnn.Execute(@"INSERT INTO CRONOLOGIA (ExpedienteId,Fecha,EstatusId,Referencia,UsuarioId,Observaciones,UsuarioIniciales,JSONDOC,HTMLDOC)
                            SELECT e.Id as ExpedienteId, FechaPublicacion, 224, 'Gaceta', 0, 'edicto-publicado', '_system', null, null
                            FROM gaceta g
                            INNER JOIN Expedientes e ON g.Numero = e.Numero and g.TipoDeRegistroId = e.TipoDeRegistroId and ModuloId=5
                            LEFT JOIN Cronologia cr ON cr.ExpedienteId = e.Id AND cr.EstatusId=224
                            WHERE g.TipoDeRegistroId=33 and cr.ExpedienteId is null");
        }
    }
}
