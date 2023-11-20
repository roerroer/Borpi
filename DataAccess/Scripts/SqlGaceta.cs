using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Scripts
{
    public class SqlGaceta{
        /*
         * Otras publicaciones en la gaceta
         */
        public static string SQL_GACETA_COUNTER = @"
            SELECT COUNT(*)
            FROM Gaceta
	        WHERE GacetaSeccionId = @GacetaSeccionId AND  FechaPublicacion <= CAST(GETDATE() as date)
        ";

        private static string SQL_GACETA_GRL = @"
            SELECT Gaceta.*
	        FROM Gaceta
            INNER JOIN Expedientes ON Expedientes.Numero = Gaceta.Numero AND Expedientes.TipoDeRegistroId = Gaceta.TipoDeRegistroId [inner-search]
	        WHERE GacetaSeccionId = @GacetaSeccionId AND FechaPublicacion <= CAST(GETDATE() as date) [date-clause]
	        ORDER BY [order-rank] FechaPublicacion desc
            OFFSET @pageSize * (@pageNumber - 1) ROWS
            FETCH NEXT @pageSize ROWS ONLY";

        /* END: EDICTOS DE PATENTES PARA LA GACETA */

        //
        // Edictos con paginacion 
        //
        public static string get_SQL_GACETA_GRL() {
            return SQL_GACETA_GRL
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "");
        }

        private static string text_filter = @"INNER JOIN CONTAINSTABLE (patentes.Patentes, Descripcion, '""*[filter]*""') AS KEY_TBL ON  Patentes.Id = KEY_TBL.[KEY]";

        //
        // Edictos con paginacion y filtro
        //
        public static string get_SQL_GACETA_GRL_TEXT_FILTER(String text)
        {
            return SQL_GACETA_GRL
                .Replace("[inner-search]", text_filter.Replace("[filter]", text))
                .Replace("[date-clause]", "")
                .Replace("[order-rank]", "KEY_TBL.RANK asc,");
        }

        //
        // Edictos con paginacion y fecha de publicacion
        //
        public static string get_SQL_GACETA_GRL_DATE_FILTER()
        {
            return SQL_GACETA_GRL
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND FechaPublicacion = DATEADD(dd, DATEDIFF(dd, 0, @fechaPublicacion), 0)")
                .Replace("[order-rank]", "");
        }

        //
        // Edictos con paginacion y fecha de publicacion
        //
        public static string get_SQL_GACETA_GRL_SEMANAL()
        {
            return SQL_GACETA_GRL
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (FechaPublicacion >= @from AND FechaPublicacion <= @to)")
                .Replace("[order-rank]", "");
        }

        public static string get_SQL_GACETA_GRL_ID()
        {
            return SQL_GACETA_GRL
                .Replace("[inner-search]", "")
                .Replace("[date-clause]", "AND (Gaceta.Id = @Id)")
                .Replace("[order-rank]", "");
        }
    }
}
