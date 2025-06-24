using System.ComponentModel.DataAnnotations;

namespace TradingSystemsMonitoring.DataModel.Entities.Base
{
    /// <summary>
    /// Базовая сущность в модели данных c идентификатором.
    /// </summary>
    public class Entity<T> : BaseEntity, IEntity<T>
    {
        [Key]
        public virtual T Id { get; set; }
    }
}
