using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.Data.Base;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Repos
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
