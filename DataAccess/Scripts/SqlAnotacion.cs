using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Scripts
{
    public class SqlAnotacion
    {
        /*
         * Expedientes en anotacion
         */
        public static string SQL_EXPEDIENTES_DE_MARCA_EN_ANOTACION = @"
        SELECT a.Id,
              a.ExpedienteId,
              a.EnRegistro,
              a.Raya,
              a.EnExpedienteId,
              a.EstatusId,
              a.ActualizadoPorUsuarioId,
              a.FechaActualizacion,	  
	          --m.Registro AS AnotacionEnRegistro, 
	          em.numero AS NumeroDeExpedienteMarca, 
	          --m.Raya, 
	          tr.codigo AS TipoDeRegistroDeLaMarca, 
	          m.Denominacion, 
	          t.Nombre AS NombreTitular, 
	          crono.Fecha FechaDeRegistro, 
	          em.FechaDeEstatus FechaDeRegistroEnMarcas,
	          niza.Codigo AS ClasificacionNiza
        FROM anotacionEnExpedientes a
        INNER JOIN expedientes e ON (a.ExpedienteId = e.Id)
        INNER JOIN marcas m ON (a.EnRegistro = m.Registro)
        INNER JOIN expedientes em ON (m.ExpedienteId = em.id)
        INNER JOIN TiposDeRegistro tr ON (em.TipoDeRegistroId = tr.Id)
        LEFT JOIN ClassificacionDeNiza niza ON (m.ClassificacionDeNizaId = niza.Id)
        LEFT JOIN Cronologia crono ON (m.ExpedienteId = crono.ExpedienteId AND crono.EstatusId = 41)
        LEFT JOIN TitularesDeLaMarca titm ON (m.ExpedienteId = titm.ExpedienteId)
        LEFT JOIN Titulares t ON (titm.TitularId = t.Id)
        WHERE e.Id =  [ExpedienteId]
        ORDER BY m.Denominacion
        ";
        /* END >> SQL_EXPEDIENTES_DE_MARCA_EN_ANOTACION*/


        /*
         * ANOTACIONES PARA PUBLICACION EN LA GACETA
         */
        public static string SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_COUNTER = @"
            SELECT COUNT(*)
            FROM Cronologia
	        WHERE Cronologia.EstatusId = (SELECT Id FROM estatus where codigo = '36') AND Cronologia.Fecha >= '2014-01-01 00:00:00.000' AND Cronologia.Fecha <= CAST(GETDATE() as date)
        ";

        public static string SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA = @"
        WITH Cronologia_anotaciones_publicadas AS
          (
            SELECT Cronologia.*
	        FROM Cronologia
            INNER JOIN Expedientes ON Expedientes.Id = Cronologia.ExpedienteId
            INNER JOIN Gaceta ON Expedientes.Numero = Gaceta.Numero AND Expedientes.TipoDeRegistroId = Gaceta.TipoDeRegistroId [inner-search]
	        WHERE Cronologia.EstatusId = (SELECT Id FROM estatus where codigo = '36') AND Cronologia.Fecha >= '2014-01-01 00:00:00.000' AND Cronologia.Fecha <= GETDATE() [date-clause]
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
	        CronologiaAnotaPublicadas.ExpedienteId,
	        CronologiaAnotaPublicadas.Fecha,
	        CronologiaAnotaPublicadas.EstatusId,
	        CronologiaAnotaPublicadas.Referencia,
	        CronologiaAnotaPublicadas.UsuarioId,
	        CronologiaAnotaPublicadas.Observaciones,
	        CronologiaAnotaPublicadas.UsuarioIniciales,
	        CronologiaAnotaPublicadas.ExpedienteId,
	        Gaceta.TipoDeRegistroId,
	        Gaceta.Numero,
	        Gaceta.GacetaSeccionId,
	        Gaceta.FechaPublicacion,
	        Gaceta.JSONDOC,
	        Gaceta.enabled,
            Gaceta.HTMLDOC,
            Gaceta.FechaEdicto
        FROM Cronologia_anotaciones_publicadas CronologiaAnotaPublicadas
        INNER JOIN Expedientes ON Expedientes.Id = CronologiaAnotaPublicadas.ExpedienteId
        INNER JOIN Gaceta ON Expedientes.Numero = Gaceta.Numero AND Expedientes.TipoDeRegistroId = Gaceta.TipoDeRegistroId
        ORDER BY CronologiaAnotaPublicadas.Fecha DESC";
        /* END >> ANOTACIONES PARA PUBLICACION EN LA GACETA */

        //
        // Anotaciones para publicacion con paginacion 
        //
        public static string get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA()
        {
            return SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "");
        }

        private static string text_filter = @"INNER JOIN CONTAINSTABLE (dbo.gaceta, HTMLDOC, '""*[filter]*""') AS KEY_TBL ON  gaceta.Id = KEY_TBL.[KEY]";

        //
        // Anotaciones para publicacion con paginacion y filtro
        //
        public static string get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_TEXT_FILTER(String text)
        {
            return SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA
                .Replace("[inner-search]", text_filter.Replace("[filter]", text))
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "KEY_TBL.RANK asc,");
        }

        //
        // Anotaciones para publicacion con paginacion y fecha de publicacion
        //
        public static string get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_DATE_FILTER()
        {
            return SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND Cronologia.Fecha = DATEADD(dd, DATEDIFF(dd, 0, @fechaPublicacion), 0)")
                .Replace("[order-rank]", "");
        }

        //
        // Anotaciones para publicacion con paginacion y fecha de publicacion
        //
        public static string get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_SEMANAL()
        {
            return SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (Cronologia.Fecha >= @from AND Cronologia.Fecha <= @to)")
                .Replace("[order-rank]", "");
        }

        //
        // Anotaciones para publicacion con paginacion por expediente-id
        //
        public static string get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_BY_EXPEDIENTE_ID()
        {
            return SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (Cronologia.ExpedienteId = @expedienteId)")
                .Replace("[order-rank]", "");
        }

    }
}
