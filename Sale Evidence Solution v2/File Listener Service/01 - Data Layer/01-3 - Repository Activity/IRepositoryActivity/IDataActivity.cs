using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ADS.SaleEvidence.RetailServices.RepositoryActivity
{
    public interface IDataActivity
    {
        #region Fetch

        T GetById<T>(object[] pk, params Expression<Func<T, object>>[] includeExpressions) where T : class;
        IQueryable<T> GetAll<T>(params Expression<Func<T, object>>[] includeExpressions) where T : class;
        IEnumerable<T> GetByCriteria<T>(
            Expression<Func<T, bool>> criteria,
            params Expression<Func<T, object>>[] includeStatements
            ) where T : class;
        IEnumerable<T> GetJoinnedWithCriteria<T, K>(Func<T, bool> criteria) where T : class where K : class;

        #endregion Fetch

        #region Insert & Update

        T Save<T>(T entity, params object[] pk) where T : class;

        #endregion Insert & Update
    }
}
