using PI.Common;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IGacetaRepository : IRepository<Expediente>
    {
        //
        // Edicto de Patentes
        //
        PagedList GetPagePublicacionEdictoPatente(int pageNumber, int pageSize);
        PagedList GetPagePublicacionEdictoPatente(int pageNumber, int pageSize, string filter);
        PagedList GetPagePublicacionEdictoPatenteFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        PagedList GetPublicacionEdictoPatenteSemanal(DateTime from, DateTime to);
        ResultInfo GetPublicacionEdictoPatenteByExpedienteId(int expedienteId);

        //
        // Edicto Anotaciones de Marca
        //
        PagedList GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize);
        PagedList GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize, string filter);
        PagedList GetPagePublicacionEdictoAnotaMarcaFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        PagedList GetPublicacionEdictoAnotaMarcaSemanal(DateTime from, DateTime to);
        ResultInfo GetPublicacionEdictoAnotaMarcaByExpedienteId(int expedienteId);

        //
        // Gaceta General
        //
        PagedList GetPagePublicacionGacetaGrl(int pageNumber, int pageSize);
        PagedList GetPagePublicacionGacetaGrl(int pageNumber, int pageSize, string filter);
        PagedList GetPagePublicacionGacetaGrlFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion);
        PagedList GetPagePublicacionGacetaGrlSemanal(DateTime from, DateTime to);
        ResultInfo GetPagePublicacionGacetaGrlById(int gacetaId);
    }
}
