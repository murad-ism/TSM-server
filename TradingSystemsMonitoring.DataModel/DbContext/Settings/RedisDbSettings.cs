using System;
using Microsoft.Extensions.Configuration;

namespace TradingSystemsMonitoring.DataModel.DbContext.Settings
{
    public class RedisDbSettings
    {
        public static string ConnectionString { get; private set; }

        public static void ReadConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
        }
    }
}
