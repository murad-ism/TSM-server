using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace TradingSystemsMonitoring.DataModel.Identity
{
    public class TsmRole : IdentityRole
    {
        public TsmRole()
        {
        }
    }
    
    public struct TsmRoleNames
    {
        public const string Admin = "admin";
        public const string User = "user";
    }
}
