using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.Application.Abstractions.Identity
{
    public interface IJwtGenerator
    {
        string CreateToken(TsmUser user, string[] userRoles);
    }
}
