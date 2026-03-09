using TradingSystemsMonitoring.DataModel.Entities.Kafka;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Services.DTO
{
    /// <summary>
    /// Торговая сделка.
    /// </summary>
    public class AccountClosedTradeDTO
    {
        public long Id { get; set; }

        /// <summary>
        /// Account код.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Trading system идентификатор.
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// Инструмент
        /// </summary>
        public string Security { get; set; }
        /// <summary>
        /// Дата открытия
        /// </summary>
        public string OpeningDate { get; set; }
        /// <summary>
        /// Дата закрытия
        /// </summary>
        public string ClosingDate { get; set; }
        /// <summary>
        /// Цена открытия (средняя)
        /// </summary>
        public double OpeningPrice { get; set; }
        /// <summary>
        /// Цена закрытия (средняя)
        /// </summary>
        public double ClosingPrice { get; set; }
        /// <summary>
        /// Размер сделки
        /// </summary>
        public long Quantity { get; set; }
        /// <summary>
        /// Направление сделки
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// Код фьючерсного контракта, если есть
        /// </summary>
        public string FutCode { get; set; }
        /// <summary>
        /// Итоговый результат сделки (в тиках)
        /// </summary>
        public string ResultInTicks { get; set; }
        /// <summary>
        /// Итоговый результат сделки (в %)
        /// </summary>
        public string ResultInPercent { get; set; }
        /// <summary>
        /// Итоговый результат сделки (в деньгах)
        /// </summary>
        public string ResultInCash { get; set; }

        public static AccountClosedTradeDTO MapFromTrade(AccountClosedTrade trade)
        {
            return new AccountClosedTradeDTO
            {
                Id = trade.Id,
                SystemId = trade.SystemId,
                OpeningDate = $"{trade.OpeningDate:dd.MM.yyyy}",
                ClosingDate = $"{trade.ClosingDate:dd.MM.yyyy}",
                Security = trade.Security?.Ticker,
                AccountId = trade.AccountId,
                ClosingPrice = trade.ClosingPrice,
                OpeningPrice = trade.OpeningPrice,
                FutCode = trade.FutCode,
                Operation = trade.Operation == 1 ? "BUY" : "SELL",
                Quantity = trade.Quantity,
                ResultInCash = $"{trade.ResultInCash:0.00}",
                ResultInPercent = $"{trade.ResultInPercent:0.00}",
                ResultInTicks = $"{trade.ResultInTicks:0.00}"
            };
        }

        public static AccountClosedTradeDTO MapFromDeal(TradeDealResult trade)
        {
            return new AccountClosedTradeDTO
            {
                Id = long.Parse(trade.Id),
                SystemId = trade.System,
                OpeningDate = $"{trade.OpenDateTime:dd.MM.yyyy}",
                ClosingDate = trade.CloseDateTime.HasValue ? $"{trade.CloseDateTime.Value:dd.MM.yyyy}" : null,
                Security = trade.Security?.Ticker,
                AccountId = trade.Account,
                ClosingPrice = trade.ClosePrice,
                OpeningPrice = trade.OpenPrice,
                Operation = trade.Operation == TradeOperation.Buy ? "BUY" : "SELL",
                Quantity = trade.Quantity,
                ResultInCash = $"{trade.TotalInCash:0.00}",
                ResultInPercent = $"{trade.TotalInPercent:0.00}",
                ResultInTicks = $"{trade.TotalInTicks:0.00}"
            };
        }

        
    }
}
