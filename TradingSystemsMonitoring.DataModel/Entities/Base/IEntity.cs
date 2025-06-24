namespace TradingSystemsMonitoring.DataModel.Entities.Base
{
    /// <summary>
    /// Базовая сущность в модели данных с полем идентификатора.
    /// </summary>
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
