using System;
using TradingSystemsMonitoring.DataModel.Entities.Base;

namespace TradingSystemsMonitoring.DataModel.Entities.Trading
{
    /// <summary>
    /// Завершенные сделки
    /// </summary>
    public class AccountClosedTrade : Entity<long>
    {
        /// <summary>
        /// Аккаунт
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Идентификатор торговой системы
        /// </summary>
        public string SystemId { get; set; }
        
        /// <summary>
        /// Инструмент
        /// </summary>
        public long fk_SecurityId { get; set; }

        public virtual Security Security { get; set; }
        
        /// <summary>
        /// Дата открытия
        /// </summary>
        public DateTime? OpeningDate { get; set; }

        /// <summary>
        /// Дата закрытия
        /// </summary>
        public DateTime? ClosingDate { get; set; }


        /// <summary>
        /// Цена открытия (средняя)
        /// </summary>
        public double OpeningPrice { get; set; }

        /// <summary>
        /// Цена закрытия (средняя)
        /// </summary>
        public double ClosingPrice { get; set; }

        /// <summary>
        /// Объем сделки
        /// </summary>
        public long Quantity { get; set; }

        /// <summary>
        /// Направление сделки
        /// </summary>
        public byte Operation { get; set; }

        public string FutCode { get; set; }

        /// <summary>
        /// Итоговый результат сделки
        /// </summary>
        public double ResultInTicks { get; set; }
        public double ResultInPercent { get; set; }
        public double ResultInCash { get; set; }

    }
}
