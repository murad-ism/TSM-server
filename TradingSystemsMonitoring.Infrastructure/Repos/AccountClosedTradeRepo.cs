using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.Application.DTO;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.Infrastructure.Base;

namespace TradingSystemsMonitoring.Infrastructure.Repos
{
    /// <summary>
    /// Репозиторий для работы с совершенными сделками торговой системы.
    /// </summary>
    public class AccountClosedTradesRepo : Repo<AccountClosedTrade>
    {
        public AccountClosedTradesRepo(DbContext context) : base(context) { }

        public override AccountClosedTrade Create()
        {
            return new AccountClosedTrade();
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
            int? pageSize, int? pageIndex)
        {
            var query = SearchByParamsQuery(systemId, securityId, from, to);
            query = query.OrderByDescending(x => x.ClosingDate);

            if (pageIndex != null && pageSize != null)
            {
                query = query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
            }

            var trades = await query.ToListAsync();
            return trades.Select(AccountClosedTradeDTO.MapFromTrade);
        }

        /// <summary>
        /// Получить запрос поиска торговых сделок по параметрам.
        /// </summary>
        /// <param name="systemId">Идентификатор торговой системы.</param>
        /// <param name="securityId">Идентификатор торгового инструмента.</param>
        /// <param name="from">От какой даты.</param>
        /// <param name="to">По какую дату.</param>
        /// <returns>Запрос возвращающий коллекцию элеметов типа <see cref="AccountClosedTrade"/>.</returns>
        public IQueryable<AccountClosedTrade> SearchByParamsQuery(
            string systemId, long? securityId, DateTime? from, DateTime? to)
        {
            var query = EntitySet.Include(x => x.Security).AsQueryable();

            if (!string.IsNullOrEmpty(systemId))
            {
                query = query.Where(x => x.SystemId == systemId);
            }

            if (securityId != null)
            {
                query = query.Where(x => x.fk_SecurityId == securityId);
            }

            if (from != null)
            {
                query = query.Where(x => x.OpeningDate >= from);
            }

            if (to != null)
            {
                query = query.Where(x => x.OpeningDate <= to);
            }

            return query;
        }

        /// <summary>
        /// Получить количество торговых сделок по параметрам.
        /// </summary>
        /// <param name="systemId">Идентификатор торговой системы.</param>
        /// <param name="securityId">Идентификатор торгового инструмента.</param>
        /// <param name="from">От какой даты.</param>
        /// <param name="to">По какую дату.</param>
        /// <returns>Количество сделок, удовлетворяющих условиями поиска.</returns>
        public async Task<int> GetCountByParams(string systemId, long? securityId, DateTime? from, DateTime? to)
        {
            var query = SearchByParamsQuery(systemId, securityId, from, to);
            return await query.CountAsync();
        }

        /// <summary>
        /// Получить список уникальных кодов всех акканутов по всем совершенным сделкам.
        /// </summary>
        /// <returns>Список кодов всех акканутов.</returns>
        public async Task<string[]> GetAccountIds()
        {
            return await GetAll().Select(x => x.AccountId).Distinct().ToArrayAsync();
        }

        /// <summary>
        /// Получить список уникальных кодов всех торговых систем по всем совершенным сделкам.
        /// </summary>
        /// <returns>Список кодов всех акканутов.</returns>
        public async Task<string[]> GetSystemIds()
        {
            return await GetAll().Select(x => x.SystemId).Distinct().ToArrayAsync();
        }
    }
}


