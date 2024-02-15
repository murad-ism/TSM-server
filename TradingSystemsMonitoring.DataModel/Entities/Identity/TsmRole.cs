using Microsoft.AspNetCore.Identity;

namespace TradingSystemsMonitoring.DataModel.Entities.Identity
{
    public class TsmRole : IdentityRole
    {
        public TsmRole() { }
    }

    public struct TsmRoleNames
    {
        public const string Admin = "admin";
        public const string User = "user";
    }
}
