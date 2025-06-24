using System;
using Microsoft.Extensions.Configuration;

namespace TradingSystemsMonitoring.DataModel.DbContext.Settings
{
    public class TradingDataDbSettings
    {
        public const string ConnectionStringName = "TradingDataDbConnection";
        public static string ConnectionString { get; private set; }

        public static void ReadConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            var connectionString = configuration.GetConnectionString(ConnectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"{ConnectionStringName} is empty in configuration file!");
            }

            ConnectionString = connectionString;
        }
    }
}
