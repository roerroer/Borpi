using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using PI.Common;

namespace PI.DataAccess
{
    public class Transaction : ITransaction
    {
        //private readonly IDatabaseFactory databaseFactory;
        private GPIEntities _context;

        public Transaction(IDatabaseFactory databaseFactory)
        {
            _context = _context ?? databaseFactory.MakeDbContext();
        }
        
        protected GPIEntities DataContext
        {
            get { return _context; }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        //public ResultInfo 
        public TServiceResult ExecuteInTransactionScope<TServiceResult>(Func<TServiceResult> script) where TServiceResult : class, new()
        {
            TServiceResult serviceResult = default(TServiceResult);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    serviceResult = script();
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (DbEntityValidationException e)
                {
                    if (e.EntityValidationErrors != null) {
                        foreach (var entityValidationError in e.EntityValidationErrors)
                        {
                            LOGGER.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                entityValidationError.Entry.Entity.GetType().Name, entityValidationError.Entry.State));

                            if (entityValidationError.ValidationErrors != null) {
                                foreach (var validationError in entityValidationError.ValidationErrors)
                                {
                                    LOGGER.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"", validationError.PropertyName, validationError.ErrorMessage));
                                }
                            }
                        }
                    }

                    LOGGER.Error(e.Message);
                    transaction.Rollback();
                }
            }

            return serviceResult;
        }


    }
}
