using System.ComponentModel.DataAnnotations;

namespace TradingSystemsMonitoring.DataModel.Entities.Base
{
    public class BaseEntity { }

    /// <summary>
    /// Базовая сущность
    /// </summary>
    public class Entity<T> : BaseEntity, IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }
    }
}
