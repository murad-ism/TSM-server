using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.Infrastructure.Base;

namespace TradingSystemsMonitoring.Infrastructure.Repos
{
    /// <summary>
    /// Репозиторий для работы с торговыми инструментами.
    /// </summary>
    public class SecurityRepo : Repo<Security>
    {
        public SecurityRepo(DbContext context)
            : base(context) { }

        public override Security Create()
        {
            return new Security();
        }

        /// <summary>
        /// Получить список всех торговых инструментов.
        /// </summary>
        /// <returns>Коллекция элементов типа<see cref="Security"/>.</returns>
        public async Task<Security[]> GetSecurities()
        {
            return await GetAll().ToArrayAsync();
        }
    }
}


