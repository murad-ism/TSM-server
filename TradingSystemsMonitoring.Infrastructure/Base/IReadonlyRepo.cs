using System;
using System.Linq;
using System.Linq.Expressions;

namespace TradingSystemsMonitoring.Infrastructure.Base
{
    /// <summary>
    /// Репозиторий на чтение для работы с данными.
    /// </summary>
    /// <typeparam name="T">Тип модели данных (Entity).</typeparam>
    public interface IReadonlyRepo<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}


