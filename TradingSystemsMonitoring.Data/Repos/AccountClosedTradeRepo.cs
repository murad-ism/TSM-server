using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.Data.Base;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Repos
{
    public class AccountClosedTradesRepo : Repo<AccountClosedTrade>
    {
        public AccountClosedTradesRepo(DbContext context) : base(context) { }

        public override AccountClosedTrade Create()
        {
            return new AccountClosedTrade();
        }

        public IEnumerable<AccountClosedTrade> GetAllWithSecurities()
        {
            return EntitySet.Include(x => x.Security).ToArray();
        }

        private IQueryable<AccountClosedTrade> SearchByParamsQuery(
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

        public async Task<int> CountByParams(string systemId, long? securityId, DateTime? from, DateTime? to)
        {
            var query = SearchByParamsQuery(systemId, securityId, from, to);
            return await query.CountAsync();
        }

        public async Task<string[]> GetAccountIds()
        {
            return await GetAll().Select(x => x.AccountId).Distinct().ToArrayAsync();
        }

        public async Task<string[]> GetSystemIds()
        {
            return await GetAll().Select(x => x.SystemId).Distinct().ToArrayAsync();
        }
    }
}
