using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IGacetaManager : IManager<Expediente>
    {
        //
        // Edicto de Patentes
        //
        ResultInfo GetPagePublicacionEdictoPatente(int pageNumber, int pageSize);
        ResultInfo GetPagePublicacionEdictoPatente(int pageNumber, int pageSize, string filter);
        ResultInfo GetPagePublicacionEdictoPatenteFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        ResultInfo GetPublicacionEdictoPatenteSemanal(DateTime from, DateTime to);
        ResultInfo GetPublicacionEdictoPatenteByExpedienteId(int expedienteId);

        //
        // Edicto Anotaciones de Marca
        //
        ResultInfo GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize);
        ResultInfo GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize, string filter);
        ResultInfo GetPagePublicacionEdictoAnotaMarcaFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        ResultInfo GetPublicacionEdictoAnotaMarcaSemanal(DateTime from, DateTime to);
        ResultInfo GetPublicacionEdictoAnotaMarcaByExpedienteId(int expedienteId);


        //
        // Gaceta General
        //
        ResultInfo GetPagePublicacionGacetaGrl(int pageNumber, int pageSize);
        ResultInfo GetPagePublicacionGacetaGrl(int pageNumber, int pageSize, string filter);
        ResultInfo GetPagePublicacionGacetaGrlFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        ResultInfo GetPagePublicacionGacetaGrlSemanal(DateTime from, DateTime to);
        ResultInfo GetPagePublicacionGacetaGrlById(int gacetaId);
    }
}
