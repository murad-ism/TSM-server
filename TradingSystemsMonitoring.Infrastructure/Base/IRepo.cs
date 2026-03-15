using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.Infrastructure.Base
{
    /// <summary>
    /// Репозиторий для работы с данными.
    /// </summary>
    /// <typeparam name="T">Тип модели данных (Entity).</typeparam>
    public interface IRepo<T> : IReadonlyRepo<T> where T : class
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add(T entity);
        T Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}


