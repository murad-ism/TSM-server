using System;
using System.Linq;
using System.Linq.Expressions;
using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.Data.Base
{
    public interface IRepo<T> : IReadonlyRepo<T> where T : class
    {
        T Add(T entity);
        T Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}
