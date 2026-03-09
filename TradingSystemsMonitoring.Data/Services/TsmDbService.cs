using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Data.Repos;
using TradingSystemsMonitoring.Data.Abstractions;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.Entities.Kafka;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Services
{
    /// <summary>
    /// Сервис для работы с совершенными сделками торговой системы.
    /// </summary>
    public class AccountClosedTradesService : IAccountClosedTradesService
    {
        private readonly IDbContextFactory<TradingDataDbContext> _tradingDataDbContextFactory;
        private readonly IConnectionMultiplexer _redis;

        public AccountClosedTradesService(IDbContextFactory<TradingDataDbContext> factory, IConnectionMultiplexer redis)
        {
            _tradingDataDbContextFactory = factory;
            _redis = redis;
        }

        /// <summary>
        /// Поиск торговых сделок по параметрам.
        /// </summary>
        /// <param name="systemId">Идентификатор торговой системы.</param>
        /// <param name="securityId">Идентификатор торгового инструмента.</param>
        /// <param name="from">От какой даты.</param>
        /// <param name="to">По какую дату.</param>
        /// <param name="pageSize">Размер выдачи на странице.</param>
        /// <param name="pageIndex">Номер страницы.</param>
        /// <returns>Коллекция элеметов типа <see cref="AccountClosedTradeDTO"/>.</returns>
        public async Task<IEnumerable<AccountClosedTradeDTO>> SearchByParams(
            string systemId, long? securityId, DateTime? from, DateTime? to,
            int? pageIndex, int? pageSize)
        {
            using (var dbContext = await _tradingDataDbContextFactory.CreateDbContextAsync())
            {
                var repo = new AccountClosedTradesRepo(dbContext);
                return await repo.SearchByParams(systemId, securityId, from, to, pageSize, pageIndex);
            }
        }

        /// <summary>
        /// Получить Поличество торговых сделок по параметрам.
        /// </summary>
        /// <param name="systemId">Идентификатор торговой системы.</param>
        /// <param name="securityId">Идентификатор торгового инструмента.</param>
        /// <param name="from">От какой даты.</param>
        /// <param name="to">По какую дату.</param>
        /// <returns>Количество элеметов типа <see cref="AccountClosedTrade"/>.</returns>
        public async Task<int> CountByParams(
            string systemId, long? securityId, DateTime? from, DateTime? to)
        {
            using (var dbContext = await _tradingDataDbContextFactory.CreateDbContextAsync())
            {
                var repo = new AccountClosedTradesRepo(dbContext);
                return await repo.GetCountByParams(systemId, securityId, from, to);
            }
        }

        /// <summary>
        /// Получить список уникальных кодов всех акканутов по всем совершенным сделкам.
        /// </summary>
        /// <returns>Список кодов всех акканутов.</returns>
        public async Task<string[]> GetAccountIds()
        {
            using (var dbContext = await _tradingDataDbContextFactory.CreateDbContextAsync())
            {
                var repo = new AccountClosedTradesRepo(dbContext);
                return await repo.GetAccountIds();
            }
        }

        /// <summary>
        /// Получить список уникальных кодов всех торговых систем по всем совершенным сделкам.
        /// </summary>
        /// <returns>Список кодов всех акканутов.</returns>
        public async Task<string[]> GetSystemIds()
        {
            using (var dbContext = await _tradingDataDbContextFactory.CreateDbContextAsync())
            {
                var repo = new AccountClosedTradesRepo(dbContext);
                return await repo.GetSystemIds();
            }
        }

        public async Task<List<AccountClosedTradeDTO>> GetCurrentTradesAsync()
        {
            var db = _redis.GetDatabase();
            var result = new List<AccountClosedTradeDTO>();

            // Получаем все trade IDs из индексного SET
            var tradeIds = await db.SetMembersAsync("trades:index");

            foreach (var id in tradeIds)
            {
                var tradeKey = $"trade:{id}";
                var json = await db.StringGetAsync(tradeKey);

                if (json.IsNullOrEmpty)
                    continue;

                var trade = JsonConvert.DeserializeObject<TradeDealResult>(json!);
                if (trade != null)
                    result.Add(AccountClosedTradeDTO.MapFromDeal(trade));
            }

            return result;
        }
    }
}
