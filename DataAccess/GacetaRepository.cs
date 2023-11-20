using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using PI.Models.Composite;
using PI.Common;
using System.Data.SqlClient;
using Dapper;
using PI.DataAccess.Scripts;
using PI.Models.Enums;

namespace PI.DataAccess
{
    public class GacetaRepository : Repository<Expediente>, IGacetaRepository
    {
        public GacetaRepository(IDatabaseFactory dbFactory) : base(dbFactory) { }
        private Dictionary<int, SnapshotPatente> dictionary = new Dictionary<int, SnapshotPatente>();
        private Dictionary<int, SnapshotAnotacion> dicAnotaMarcas = new Dictionary<int, SnapshotAnotacion>();

        //
        // Patentes: Edictos
        //
        public PagedList GetPagePublicacionEdictoPatente(int pageNumber, int pageSize) {

            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotPatente, Patente, Cronologia, IPC, Agente, Gaceta, SnapshotPatente>
                            (
                                SqlPatentes.get_SQL_EDICTO_DE_PATENTES(),
                                expPatenteParser,
                                new { pageNumber = pageNumber, pageSize = pageSize }, 
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlPatentes.SQL_EDICTO_DE_PATENTES_COUNTER).Single();

                    var resultSet = dictionary.Values.ToList();
                    result.DataSet = additionalEntities(conn, resultSet);

                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        private List<SnapshotPatente> additionalEntities(SqlConnection conn, List<SnapshotPatente> result) {
            
            foreach (var i in result) {
                i.inventores = GetInventores(conn, i.Id);
                i.prioridades = GetPrioridades(conn, i.Id);
                i.titulares = GetTitulares(conn, i.Id);
            }
            return result;
        }

        private SnapshotPatente expPatenteParser(SnapshotPatente snapshotPatente, Patente patente, Cronologia cronologia, IPC classificationIPC, Agente agente, Gaceta gaceta)
        {
            SnapshotPatente newEntry;

            if (!dictionary.TryGetValue(snapshotPatente.Id, out newEntry))
            {
                newEntry = snapshotPatente;
                newEntry.Patente = patente;
                newEntry.agente = agente;
                newEntry.Gaceta = gaceta;
                newEntry.cronologia = cronologia;

                //Init collections
                newEntry.classificationIPC = new List<IPC>();
                dictionary.Add(snapshotPatente.Id, newEntry);
            }

            //
            // only for collections
            //
            if (classificationIPC!=null)
                newEntry.classificationIPC.Add(classificationIPC);
            return newEntry;
        }

        public List<vInventorDeLaPatente> GetInventores(SqlConnection conn, int expedienteId)
        {
            var dataset = conn.Query<vInventorDeLaPatente>(SqlPatentes.GET_SQL_INVENTORES, new { ExpedienteId = expedienteId });

            return dataset.ToList();
        }


        public List<vPrioridadDeLaPatente> GetPrioridades(SqlConnection conn, int expedienteId)
        {
            var dataset = conn.Query<vPrioridadDeLaPatente>(SqlPatentes.GET_SQL_PRIORIDADES, new { ExpedienteId = expedienteId });
            return dataset.ToList();
        }

        public List<vTitularDeLaPatente> GetTitulares(SqlConnection conn, int expedienteId)
        {
            var dataset = conn.Query<vTitularDeLaPatente>(SqlPatentes.GET_SQL_TITULARES, new { ExpedienteId = expedienteId });
            return dataset.ToList();
        }

        //
        // Patentes: Edictos
        //
        public PagedList GetPagePublicacionEdictoPatente(int pageNumber, int pageSize, string filter)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotPatente, Patente, Cronologia, IPC, Agente, Gaceta, SnapshotPatente>
                            (
                                SqlPatentes.get_SQL_EDICTO_DE_PATENTES_TEXT_FILTER(filter),
                                expPatenteParser,
                                new { pageNumber = pageNumber, pageSize = pageSize },
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlPatentes.SQL_EDICTO_DE_PATENTES_COUNTER).Single();

                    var resultSet = dictionary.Values.ToList();
                    result.DataSet = additionalEntities(conn, resultSet);

                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Patentes: Edictos
        //
        public PagedList GetPagePublicacionEdictoPatenteFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotPatente, Patente, Cronologia, IPC, Agente, Gaceta, SnapshotPatente>
                            (
                                SqlPatentes.get_SQL_EDICTO_DE_PATENTES_DATE_FILTER(),
                                expPatenteParser,
                                new { pageNumber = pageNumber, pageSize = pageSize, fechaPublicacion = fechaPublicacion },
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlPatentes.SQL_EDICTO_DE_PATENTES_COUNTER).Single();

                    var resultSet = dictionary.Values.ToList();
                    result.DataSet = additionalEntities(conn, resultSet);

                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Patentes: Edictos
        //
        public PagedList GetPublicacionEdictoPatenteSemanal(DateTime from, DateTime to)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotPatente, Patente, Cronologia, IPC, Agente, Gaceta, SnapshotPatente>
                            (
                                SqlPatentes.get_SQL_EDICTO_DE_PATENTES_SEMANAL(),
                                expPatenteParser,
                                new { pageNumber = 1, pageSize = 200, from = from, to = to },
                                splitOn: "ExpedienteId"
                            );

                    var resultSet = dictionary.Values.ToList();
                    result.DataSet = additionalEntities(conn, resultSet);
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Patentes: Edictos
        //
        public ResultInfo GetPublicacionEdictoPatenteByExpedienteId(int expedienteId)
        {
            var result = new ResultInfo();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotPatente, Patente, Cronologia, IPC, Agente, Gaceta, SnapshotPatente>
                            (
                                SqlPatentes.get_SQL_EDICTO_DE_PATENTES_BY_EXPEDIENTE_ID(),
                                expPatenteParser,
                                new { pageNumber = 1, pageSize = 1, expedienteId = expedienteId},
                                splitOn: "ExpedienteId"
                            );

                    var resultSet = dictionary.Values.ToList();

                    var exp = additionalEntities(conn, resultSet).FirstOrDefault();
                    result.Result = exp;
                }
            }
            catch (Exception exception)
            {
                result.Result = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }




        //
        // Marcas: Anotaciones
        //
        public PagedList GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotAnotacion, Cronologia, Gaceta, SnapshotAnotacion>
                            (
                                SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA(),
                                expMarcasParser,
                                new { pageNumber = pageNumber, pageSize = pageSize },
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlAnotacion.SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_COUNTER).Single();

                    result.DataSet = dicAnotaMarcas.Values.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }


        private SnapshotAnotacion expMarcasParser(SnapshotAnotacion snapshotMarca, Cronologia cronologia, Gaceta gaceta)
        {
            SnapshotAnotacion newEntry;

            if (!dicAnotaMarcas.TryGetValue(snapshotMarca.Id, out newEntry))
            {
                newEntry = snapshotMarca;
                newEntry.cronologia = cronologia;
                newEntry.Gaceta = gaceta;

                //Init collections
                dicAnotaMarcas.Add(snapshotMarca.Id, newEntry);
            }

            //
            // only for collections
            //
            return newEntry;
        }

        //
        // Marcas: Anotaciones
        //
        public PagedList GetPagePublicacionEdictoAnotaMarca(int pageNumber, int pageSize, string filter)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotAnotacion, Cronologia, Gaceta, SnapshotAnotacion>
                            (
                                SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_TEXT_FILTER(filter),
                                expMarcasParser,
                                new { pageNumber = pageNumber, pageSize = pageSize },
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlAnotacion.SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_COUNTER).Single();

                    result.DataSet = dicAnotaMarcas.Values.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Marcas: Anotaciones
        //
        public PagedList GetPagePublicacionEdictoAnotaMarcaFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotAnotacion, Cronologia, Gaceta, SnapshotAnotacion>
                            (
                                SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_DATE_FILTER(),
                                expMarcasParser,
                                new { pageNumber = pageNumber, pageSize = pageSize, fechaPublicacion = fechaPublicacion },
                                splitOn: "ExpedienteId"
                            );

                    int totalnRecords = conn.Query<int>(
                        SqlAnotacion.SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_COUNTER).Single();

                    result.DataSet = dicAnotaMarcas.Values.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Marcas: Anotaciones
        //
        public PagedList GetPublicacionEdictoAnotaMarcaSemanal(DateTime from, DateTime to)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotAnotacion, Cronologia, Gaceta, SnapshotAnotacion>
                            (
                                SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_SEMANAL(),
                                expMarcasParser,
                                new { pageNumber = 1, pageSize = 200, from = from, to = to },
                                splitOn: "ExpedienteId"
                            );

                    result.DataSet = dicAnotaMarcas.Values.ToList();
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Marcas: Anotaciones
        //
        public ResultInfo GetPublicacionEdictoAnotaMarcaByExpedienteId(int expedienteId)
        {
            var result = new ResultInfo();
            try
            {
                System.Diagnostics.Debug.WriteLine(SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_BY_EXPEDIENTE_ID());

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<SnapshotAnotacion, Cronologia, Gaceta, SnapshotAnotacion>
                            (
                                SqlAnotacion.get_SQL_ANOTA_MARCA_PUBLICADAS_EN_GACETA_BY_EXPEDIENTE_ID(),
                                expMarcasParser,
                                new { pageNumber = 1, pageSize = 1, expedienteId = expedienteId },
                                splitOn: "ExpedienteId"
                            );

                    var exp = dicAnotaMarcas.FirstOrDefault();
                    result.Result = exp.Value;
                }
            }
            catch (Exception exception)
            {
                result.Result = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }



        //
        // Gaceta General
        //        
        public PagedList GetPagePublicacionGacetaGrl(int pageNumber, int pageSize)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<Gaceta>
                            (
                                SqlGaceta.get_SQL_GACETA_GRL(),
                                new { pageNumber = pageNumber, pageSize = pageSize, GacetaSeccionId=4 }
                            );

                    int totalnRecords = conn.Query<int>(SqlGaceta.SQL_GACETA_COUNTER, new { GacetaSeccionId = 4 }).Single();

                    result.DataSet = dataset.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }


        //
        // Gaceta General
        //
        public PagedList GetPagePublicacionGacetaGrl(int pageNumber, int pageSize, string filter)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<Gaceta>
                            (
                                SqlGaceta.get_SQL_GACETA_GRL_TEXT_FILTER(filter),
                                new { pageNumber = pageNumber, pageSize = pageSize, GacetaSeccionId = 4 }
                            );

                    int totalnRecords = conn.Query<int>(SqlGaceta.SQL_GACETA_COUNTER, new { GacetaSeccionId = 4 }).Single();

                    result.DataSet = dataset.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Gaceta General
        //
        public PagedList GetPagePublicacionGacetaGrlFilterByDate(int pageNumber, int pageSize, DateTime fechaPublicacion)
        {
            var result = new PagedList();
            try
            {

                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<Gaceta>
                            (
                                SqlGaceta.get_SQL_GACETA_GRL_DATE_FILTER(),
                                new { pageNumber = pageNumber, pageSize = pageSize, fechaPublicacion = fechaPublicacion, GacetaSeccionId = 4 }
                            );

                    int totalnRecords = conn.Query<int>(SqlGaceta.SQL_GACETA_COUNTER, new { GacetaSeccionId = 4 }).Single();

                    result.DataSet = dataset.ToList();
                    result.TotalItems = totalnRecords;
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Gaceta General
        //
        public PagedList GetPagePublicacionGacetaGrlSemanal(DateTime from, DateTime to)
        {
            var result = new PagedList();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<Gaceta>
                            (
                                SqlGaceta.get_SQL_GACETA_GRL_SEMANAL(),
                                new { pageNumber = 1, pageSize = 200, from = from, to = to, GacetaSeccionId = 4 }
                            );

                    result.DataSet = dataset.ToList();
                }
            }
            catch (Exception exception)
            {
                result.DataSet = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }

        //
        // Gaceta General
        //
        public ResultInfo GetPagePublicacionGacetaGrlById(int gacetaId)
        {
            var result = new ResultInfo();
            try
            {
                using (SqlConnection conn = (SqlConnection)DbContext.Database.Connection)
                {
                    var dataset = conn
                            .Query<Gaceta>
                            (
                                SqlGaceta.get_SQL_GACETA_GRL_ID(),
                                new { pageNumber = 1, pageSize = 1, Id = gacetaId, GacetaSeccionId = 4 }
                            );

                    var exp = dataset.FirstOrDefault();
                    result.Result = exp;
                }
            }
            catch (Exception exception)
            {
                result.Result = exception.Message;
                LOGGER.Error(exception.Message);
            }
            return result;
        }



    }
}