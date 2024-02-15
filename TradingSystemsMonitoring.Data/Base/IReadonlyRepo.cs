using System;
using System.Linq;
using System.Linq.Expressions;

namespace TradingSystemsMonitoring.Data.Base
{
    public interface IReadonlyRepo<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}
