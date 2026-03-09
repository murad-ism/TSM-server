using TradingSystemsMonitoring.DataModel.Entities.Identity;

namespace TradingSystemsMonitoring.RestAPI.Abstractions.Identity
{
    public interface IJwtGenerator
    {
        string CreateToken(TsmUser user, string[] userRoles);
    }
}
