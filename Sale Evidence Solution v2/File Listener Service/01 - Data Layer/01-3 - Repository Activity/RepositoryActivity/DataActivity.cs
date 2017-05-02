using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ADS.SaleEvidence.RetailServices.DataBroker;
using ADS.SaleEvidence.RetailServices.ObjectModel;
using log4net;

namespace ADS.SaleEvidence.RetailServices.RepositoryActivity
{
    public class DataActivity : IDataActivity
    {
        #region Fields

        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SaleEvidenceDbContext _context;

        #endregion Fields

        #region Constructors

        public DataActivity()
        {
            _context = new SaleEvidenceDbContext();
        }

        #endregion Constructors

        #region Interface Implementation

        public T GetById<T>(object[] pk, params Expression<Func<T, object>>[] includeExpressions) where T : class
        {
            return _context.Set<T>().Find(pk);
        }

        public IQueryable<T> GetAll<T>(params Expression<Func<T, object>>[] includeExpressions) where T : class
        {
            var retVal = _context.Set<T>();

            if (includeExpressions.Any())
            {
                var set = includeExpressions
                  .Aggregate<Expression<Func<T, object>>, IQueryable<T>>
                    (retVal, (current, expression) => current.Include(expression));
            }

            return retVal;
        }

        public IEnumerable<T> GetByCriteria<T>(
            Expression<Func<T, bool>> criteria,
            params Expression<Func<T, object>>[] includeStatements
            ) where T : class
        {
            IQueryable<T> retVal = _context.Set<T>();

            if (criteria != null)
            {
                retVal = retVal.Where(criteria);
            }

            foreach (var includeStatement in includeStatements)
            {
                retVal = retVal.Include(includeStatement);
            }

            return retVal;
        }

        public IEnumerable<T> GetJoinnedWithCriteria<T, K>(Func<T, bool> criteria) where T : class where K : class
        {
            throw new NotImplementedException();

            //return context.Sellings
            //        .Join(context.Actions,
            //            selling => selling.Id,
            //            action => action.Id,
            //            (selling, action) => new { Selling = selling, Action = action }
            //        );
        }

        public T Save<T>(T entity, params object[] pk) where T : class
        {
            var entry = _context.Entry<T>(entity);

            switch (entry.State)
            {
                case EntityState.Added:
                    // Leave it as it is
                    break;

                case EntityState.Deleted:
                    // It's not to be deleted after all
                    entry.State = EntityState.Modified;
                    break;

                case EntityState.Detached:
                    // Let's see if it really exists in Db
                    T foundEntity = null;
                    if (pk != null && pk.Length > 0)
                    {
                        // Ako je pk tipa int proveriti da li je razlicito od 0, ukoliko nije int ulazi u blok
                        if (pk.Any(it => it != null && (it.GetType() == typeof(int) && ((int)it) > 0) || (it.GetType() != typeof(int))))
                        {
                            foundEntity = _context.Set<T>().Find(pk);
                        }
                    }
                    if (foundEntity == null)
                    {
                        entry.State = EntityState.Added;
                    }
                    else
                    {
                        entry = _context.Entry(foundEntity);
                        entry.CurrentValues.SetValues(entity);
                        entry.State = EntityState.Modified;
                    }
                    break;

                case EntityState.Modified:
                    // Leave it as it is
                    break;

                case EntityState.Unchanged:
                    // Leave it as it is
                    break;

                default:
                    break;
            }

            try
            {
                _context.SaveChanges();
            }
            //catch (System.Data.Entity.Validation.DbEntityValidationException enValEx)
            catch (Microsoft.EntityFrameworkCore.DbUpdateException enValEx)
            {
                //TODO: Do proper logging of DbContext exception!

                _logger.FatalFormat("Saving an entry of type '{0}' has failed because of entity validation.", typeof(T).FullName);
                _logger.FatalFormat("Error (StackTrace) - {0}", enValEx.StackTrace);
                _logger.FatalFormat("Error (InnerException) - {0}", enValEx.InnerException);
                _logger.FatalFormat("Error (Message) - {0}", enValEx.Message);

                throw enValEx;
            }
            catch (Exception exc) // for debugging
            {
                _logger.FatalFormat("Error (StackTrace) - {0}", exc.StackTrace);
                _logger.FatalFormat("Error (InnerException) - {0}", exc.InnerException);
                _logger.FatalFormat("Error (Message) - {0}", exc.Message);

                throw exc;
            }

            return entry.Entity;
        }

        #endregion Interface Implementation

        #region Private Methods
        


        #endregion Private Methods
    }
}
