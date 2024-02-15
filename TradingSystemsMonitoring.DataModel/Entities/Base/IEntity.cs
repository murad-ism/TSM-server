namespace TradingSystemsMonitoring.DataModel.Entities.Base
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}
