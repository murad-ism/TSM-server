using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TradingSystemsMonitoring.DataModel.DbContext.Infrastructure
{
    public class TradingLogRecordsDbSettings
    {
        public const string ConnectionStringName = "TradingLogRecordsDbConection";
        public static string Url { get; private set; }
        public static string Host { get; private set; }
        public static string Database { get; private set; }

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

            var url = new MongoUrl(connectionString);
            Url = connectionString;
            Host = url.Server.ToString();
            Database = url.DatabaseName;
        }
    }
}
