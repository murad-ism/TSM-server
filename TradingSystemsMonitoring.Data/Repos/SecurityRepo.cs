using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradingSystemsMonitoring.Data.Base;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.Data.Repos
{
    public class SecurityRepo : Repo<Security>
    {
        public SecurityRepo(DbContext context)
            : base(context) { }

        public override Security Create()
        {
            return new Security();
        }

        public async Task<Security[]> GetSecurities()
        {
            return await GetAll().ToArrayAsync();
        }
    }
}
