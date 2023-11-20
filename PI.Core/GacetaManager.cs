using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class GacetaManager : Manager<Expediente>, IGacetaManager
    {
        public GacetaManager(IGacetaRepository repository, ITransaction transaction) : base(repository, transaction) { }

        //
        //
        // Patentes
        //
        //
        public ResultInfo GetPagePublicacionEdictoPatente(int pageNumber, int pageSize)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoPatente(pageNumber, pageSize);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionEdictoPatente(int pageNumber, int pageSize, string filter)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoPatente(pageNumber, pageSize, filter);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionEdictoPatenteFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoPatenteFilterByDate(pageNumber, pageSize, fechaPublicacion);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPublicacionEdictoPatenteSemanal(DateTime from, DateTime to)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPublicacionEdictoPatenteSemanal(from, to);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPublicacionEdictoPatenteByExpedienteId(int expedienteId) {

            ResultInfo result = ((IGacetaRepository)_repository)
               .GetPublicacionEdictoPatenteByExpedienteId(expedienteId);

            return new ResultInfo() { Succeeded = true, Result = result.Result };
        }

        //
        //
        // Marcas
        //
        //
        public ResultInfo GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoAnotaMarca(pageNumber, pageSize);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize, string filter)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoAnotaMarca(pageNumber, pageSize, filter);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionEdictoAnotaMarcaFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionEdictoAnotaMarcaFilterByDate(pageNumber, pageSize, fechaPublicacion);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPublicacionEdictoAnotaMarcaSemanal(DateTime from, DateTime to)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPublicacionEdictoAnotaMarcaSemanal(from, to);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPublicacionEdictoAnotaMarcaByExpedienteId(int expedienteId)
        {

            ResultInfo result = ((IGacetaRepository)_repository)
               .GetPublicacionEdictoAnotaMarcaByExpedienteId(expedienteId);

            return new ResultInfo() { Succeeded = true, Result = result.Result };
        }



        //
        //
        // Gaceta Genral
        //
        //
        public ResultInfo GetPagePublicacionGacetaGrl(int pageNumber, int pageSize)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionGacetaGrl(pageNumber, pageSize);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionGacetaGrl(int pageNumber, int pageSize, string filter)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionGacetaGrl(pageNumber, pageSize, filter);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionGacetaGrlFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionGacetaGrlFilterByDate(pageNumber, pageSize, fechaPublicacion);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionGacetaGrlSemanal(DateTime from, DateTime to)
        {
            PagedList pagedData = ((IGacetaRepository)_repository)
                .GetPagePublicacionGacetaGrlSemanal(from, to);

            return new ResultInfo() { Succeeded = true, Result = pagedData };
        }

        public ResultInfo GetPagePublicacionGacetaGrlById(int expedienteId)
        {

            ResultInfo result = ((IGacetaRepository)_repository)
               .GetPagePublicacionGacetaGrlById(expedienteId);

            return new ResultInfo() { Succeeded = true, Result = result.Result };
        }

    }
}
