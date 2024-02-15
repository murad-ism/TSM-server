using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

namespace TradingSystemsMonitoring.DataModel.Mappings.Trading
{
    public class AccountClosedTradeMap : IEntityTypeConfiguration<AccountClosedTrade>
    {
        public void Configure(EntityTypeBuilder<AccountClosedTrade> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("AccountClosedTrades");
            builder.HasOne(x => x.Security).WithMany(y => y.AccountClosedTrades).HasForeignKey(x => x.fk_SecurityId);
        }
    }
}
