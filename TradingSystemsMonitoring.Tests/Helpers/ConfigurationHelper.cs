using Microsoft.Extensions.Configuration;

namespace TradingSystemsMonitoring.Tests.Helpers
{
    internal static class ConfigurationHelper
    {
        public static IConfiguration GetConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Docker")
            {
                return GetConfigForDockerEnv();
            }
            return GetConfigForDevelopEnv();
        }

        private static IConfiguration GetConfigForDevelopEnv()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Development.json")
                .Build();
        }

        private static IConfiguration GetConfigForDockerEnv()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.Docker.json")
                .Build();
        }
    }
}
