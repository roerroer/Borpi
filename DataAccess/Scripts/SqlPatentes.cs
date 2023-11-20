using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Scripts
{
    public class SqlPatentes    {
        /*
         * EDICTOS DE PATENTES PARA LA GACETA
         */
        public static string SQL_EDICTO_DE_PATENTES_COUNTER = @"
            SELECT COUNT(*)
            FROM Cronologia
	        WHERE Cronologia.EstatusId = (SELECT Id FROM estatus where codigo = '009') 
                    AND Cronologia.Fecha >= '2014-01-01 00:00:00.000' AND Cronologia.Fecha <= CAST(GETDATE() as date)
        ";

        private static string SQL_EDICTO_DE_PATENTES = @"
        WITH Cronologia_edictos AS
        (
            SELECT Cronologia.*
	        FROM Cronologia
            INNER JOIN Expedientes ON Expedientes.Id = Cronologia.ExpedienteId
            INNER JOIN Gaceta ON Expedientes.Numero = Gaceta.Numero AND Expedientes.TipoDeRegistroId = Gaceta.TipoDeRegistroId [inner-search]
	        WHERE Cronologia.EstatusId = (SELECT Id FROM estatus where codigo = '009') 
                    AND Cronologia.Fecha >= '2014-01-01 00:00:00.000' AND Cronologia.Fecha <= CAST(GETDATE() as date) [date-clause]
	        ORDER BY [order-rank] Cronologia.Fecha desc
            OFFSET @pageSize * (@pageNumber - 1) ROWS
            FETCH NEXT @pageSize ROWS ONLY
        )
        SELECT 
	        Expedientes.Id,
	        Expedientes.ModuloId,
	        Expedientes.TipoDeRegistroId,
	        Expedientes.Numero,
	        Expedientes.FechaDeSolicitud,
	        Expedientes.Hora,
	        Expedientes.EstatusId,
	        Expedientes.FechaDeEstatus,
	        Expedientes.LeyId,
	        Expedientes.UbicacionId,
	        Expedientes.UbicacionUsuarioId,
	        Expedientes.FechaTraslado,
	        Expedientes.ActualizadoPorUsuarioId,
	        Expedientes.FechaActualizacion,
	        Patentes.ExpedienteId,
	        Patentes.Descripcion,
	        Patentes.Registro,
	        Patentes.AgenteId,
	        Patentes.anualidades,
	        Patentes.Folio,
	        Patentes.Tomo,
	        Patentes.Resumen,
	        Patentes.ClasificacionId,
	        Patentes.RecibidoPorUsuarioId,
	        Patentes.FechaRecepcion,
	        Patentes.Pct,
	        Patentes.Fecha_Pct,
	        Patentes.Citaciones,
	        CronologiaEdictos.ExpedienteId,
	        CronologiaEdictos.Fecha,
	        CronologiaEdictos.EstatusId,
	        CronologiaEdictos.Referencia,
	        CronologiaEdictos.UsuarioId,
	        CronologiaEdictos.Observaciones,
	        CronologiaEdictos.UsuarioIniciales,
	        IPC.ExpedienteId,
	        IPC.Indice,
	        IPC.Classificacion,
			Expedientes.Id as ExpedienteId,
			agente.Nombre,
			agente.Domicilio,
			agente.Telefono,
			agente.Fax,
			agente.tmpId,
            Expedientes.Id as ExpedienteId,
	        Gaceta.TipoDeRegistroId,
	        Gaceta.Numero,
	        Gaceta.GacetaSeccionId,
	        Gaceta.FechaPublicacion,
	        Gaceta.JSONDOC,
	        Gaceta.enabled,
            Gaceta.HTMLDOC,
            Gaceta.FechaEdicto
        FROM Cronologia_edictos CronologiaEdictos
        JOIN Expedientes ON Expedientes.Id = CronologiaEdictos.ExpedienteId
        INNER JOIN Gaceta ON Expedientes.Numero = Gaceta.Numero AND Expedientes.TipoDeRegistroId = Gaceta.TipoDeRegistroId
        INNER JOIN Patentes.Patentes Patentes ON Expedientes.Id = Patentes.ExpedienteId
        LEFT JOIN Patentes.IPC IPC ON Expedientes.Id = IPC.ExpedienteId
        LEFT JOIN patentes.Agentes agente ON Patentes.AgenteId = agente.Id
        ORDER BY CronologiaEdictos.Fecha desc, IPC.Indice";

        /* END: EDICTOS DE PATENTES PARA LA GACETA */

        //
        // Edictos con paginacion 
        //
        public static string get_SQL_EDICTO_DE_PATENTES() {
            return SQL_EDICTO_DE_PATENTES
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "");
        }

        private static string text_filter = @"INNER JOIN CONTAINSTABLE (patentes.Patentes, Descripcion, '""*[filter]*""') AS KEY_TBL ON  Patentes.Id = KEY_TBL.[KEY]";

        //
        // Edictos con paginacion y filtro
        //
        public static string get_SQL_EDICTO_DE_PATENTES_TEXT_FILTER(String text)
        {
            return SQL_EDICTO_DE_PATENTES
                .Replace("[inner-search]", text_filter.Replace("[filter]", text))
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "KEY_TBL.RANK asc,");
        }

        //
        // Edictos con paginacion y fecha de publicacion
        //
        public static string get_SQL_EDICTO_DE_PATENTES_DATE_FILTER()
        {
            return SQL_EDICTO_DE_PATENTES
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND Cronologia.Fecha = DATEADD(dd, DATEDIFF(dd, 0, @fechaPublicacion), 0)")
                .Replace("[order-rank]", "");
        }

        //
        // Edictos con paginacion y fecha de publicacion
        //
        public static string get_SQL_EDICTO_DE_PATENTES_SEMANAL()
        {
            return SQL_EDICTO_DE_PATENTES
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (Cronologia.Fecha >= @from AND Cronologia.Fecha <= @to)")
                .Replace("[order-rank]", "");
        }

        public static string get_SQL_EDICTO_DE_PATENTES_BY_EXPEDIENTE_ID()
        {
            return SQL_EDICTO_DE_PATENTES
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (Cronologia.ExpedienteId = @expedienteId)")
                .Replace("[order-rank]", "");
        }


        public static string GET_SQL_INVENTORES = @"SELECT i.Id, ExpedienteId, i.Nombre, Direccion, Ciudad, PaisId, Telefono, p.Nombre as NombrePais, p.Codigo as CodigoPais
                                            FROM patentes.Inventores i
                                            LEFT JOIN paises p ON p.Id = PaisId
                                            WHERE ExpedienteId = @ExpedienteId";

        public static string GET_SQL_PRIORIDADES = @"SELECT pr.Id, ExpedienteId, PaisId, Fecha, Tipo_referencia, SolicitudP, p.Nombre as NombrePais, p.Codigo as CodigoPais
                                                    FROM patentes.prioridades pr
                                                    LEFT JOIN paises p ON p.Id = PaisId
                                                    WHERE ExpedienteId = @ExpedienteId";

        public static string GET_SQL_TITULARES = @"SELECT tpat.Nombre, txpat.Direccion, p.Codigo as CodigoPais, p.Nombre as NombrePais
                                            FROM patentes.TitularesDeLaPatente txpat
                                            INNER JOIN patentes.TitularesEnPatentes tpat ON txpat.TitularId = tpat.Id
                                            LEFT JOIN paises p ON txpat.PaisId = p.Id
                                            WHERE txpat.ExpedienteId = @ExpedienteId";
    }
}
