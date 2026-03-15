using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TradingSystemsMonitoring.Application.Abstractions;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.Infrastructure.Repos;

namespace TradingSystemsMonitoring.Infrastructure.Services
{
    /// <summary>
    /// Сервис для работы с торговыми инструментами.
    /// </summary>
    public class SecurityService : ISecurityService
    {
        private IDbContextFactory<TradingDataDbContext> _tradingDataDbContextFactory;
        public SecurityService(IDbContextFactory<TradingDataDbContext> factory)
        {
            _tradingDataDbContextFactory = factory;
        }

        /// <summary>
        /// Получить список всех торговых инструментов.
        /// </summary>
        /// <returns>Коллекция элементов типа<see cref="Security"/>.</returns>
        public async Task<Security[]> GetSecurities()
        {
            using (var dbContext = await _tradingDataDbContextFactory.CreateDbContextAsync())
            {
                var repo = new SecurityRepo(dbContext);
                return await repo.GetSecurities();
            }
        }
    }
}


