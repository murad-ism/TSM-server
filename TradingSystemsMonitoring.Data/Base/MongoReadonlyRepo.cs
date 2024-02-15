using System;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace TradingSystemsMonitoring.Data.Base
{
    public abstract class MongoReadonlyRepo<T> : IReadonlyRepo<T> where T : class
    {
        public abstract string MongoCollectionName { get; }

        public readonly IMongoCollection<T> EntitySet;

        protected MongoReadonlyRepo(IMongoDatabase database)
        {
            EntitySet = database.GetCollection<T>(MongoCollectionName);
            
        }
        
        public virtual IQueryable<T> GetAll()
        {
            return EntitySet.AsQueryable();
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return EntitySet.AsQueryable().Where(predicate);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return EntitySet.AsQueryable().FirstOrDefault(predicate);
        }

        public virtual IFindFluent<T, T> Where(FilterDefinition<T> filter)
        {
            return EntitySet.Find(filter);
        }
    }
}
