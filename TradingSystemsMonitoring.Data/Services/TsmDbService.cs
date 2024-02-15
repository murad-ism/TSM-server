using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Data.Repos;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Services
{
    public partial class TsmDbService
    {
        public AccountClosedTradesService AccountClosedTradesSvc { get; set; }
        public SecurityService SecuritySvc { get; set; }
        public TsmDbService(TradingDataDbContext tradingDataDbContext)
        {
            AccountClosedTradesSvc = new AccountClosedTradesService(tradingDataDbContext);
            SecuritySvc = new SecurityService(tradingDataDbContext);
        }
        
        public class AccountClosedTradesService
        {
            private AccountClosedTradesRepo _accountClosedTradesRepo;
            public AccountClosedTradesService(TradingDataDbContext tradingDataDbContext)
            {
                _accountClosedTradesRepo = new AccountClosedTradesRepo(tradingDataDbContext);
            }

            public async Task<IEnumerable<AccountClosedTradeDTO>> SearchByParams(
                string systemId, long? securityId, DateTime? from, DateTime? to,
                int? pageIndex, int? pageSize)
            {
                return await _accountClosedTradesRepo.SearchByParams(systemId, securityId, from, to, pageSize, pageIndex);
            }

            public async Task<int> CountByParams(
                string systemId, long? securityId, DateTime? from, DateTime? to)
            {
                return await _accountClosedTradesRepo.CountByParams(systemId, securityId, from, to);
            }

            public async Task<string[]> GetAccountIds()
            {
                return await _accountClosedTradesRepo.GetAccountIds();
            }
            
            public async Task<string[]> GetSystemIds()
            {
                return await _accountClosedTradesRepo.GetSystemIds();
            }
        }

        public class SecurityService
        {
            private SecurityRepo _securityRepo;
            public SecurityService(TradingDataDbContext tradingDataDbContext)
            {
                _securityRepo = new SecurityRepo(tradingDataDbContext);
            }

            public async Task<Security[]> GetSecurities()
            {
                return await _securityRepo.GetSecurities();
            }
        }
    }
}
