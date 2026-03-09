using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace TradingSystemsMonitoring.DataModel.DbContext.Settings
{
    public class TradingLogRecordsDbSettings
    {
        private const string _connectionStringName = "TradingLogRecordsDbConnection";

        public static MongoUrl Url { get; private set; }
        public static string Host { get; private set; }
        public static string Database { get; private set; }

        public static void ReadConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            
            var connectionString = configuration.GetConnectionString(_connectionStringName);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"{_connectionStringName} is empty in configuration file!");
            }

            Url = new MongoUrl(connectionString);
            Host = Url.Server.ToString();
            Database = Url.DatabaseName;
        }
    }
}
