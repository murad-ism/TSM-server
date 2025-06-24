using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.Data.Base
{
    /// <summary>
    /// Базовый класс репозитория для работы с данными.
    /// </summary>
    /// <typeparam name="T">Тип модели данных (Entity).</typeparam>
    public abstract class Repo<T> : IRepo<T> where T : BaseEntity
    {
        protected DbContext Context;
        protected readonly DbSet<T> EntitySet;
        protected Repo(DbContext context)
        {
            Context = context;
            EntitySet = context.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return EntitySet.AsQueryable();
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = EntitySet.Where(predicate);
            return query;
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var entity = EntitySet.Where(predicate).FirstOrDefault();
            return entity;
        }

        public abstract T Create();

        public virtual T Add(T entity)
        {
            if (Context.Entry(entity).State != EntityState.Detached)
            {
                Context.Entry(entity).State = EntityState.Modified;
                return entity;
            }
            return EntitySet.Add(entity).Entity;
        }

        public virtual T Delete(T entity)
        {
            EntitySet.Attach(entity);
            return EntitySet.Remove(entity).Entity;
        }

        public virtual void Edit(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }
        
        public virtual void Save()
        {
            Context.SaveChanges();
        }
    }
}
