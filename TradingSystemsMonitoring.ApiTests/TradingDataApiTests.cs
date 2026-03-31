using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using TradingSystemsMonitoring.RestAPI;

namespace TradingSystemsMonitoring.ApiTests
{
    [TestFixture]
    public class TradingDataApiTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        [OneTimeSetUp]
        public void Setup()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => builder
                    .UseEnvironment(env));
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions());
        }

        [Test]
        public void GetSecurities_ReturnsNotEmptySecurities()
        {
            var requestUrl = "/api/TradingData/Securities";
            var response = _client.GetAsync(requestUrl).GetAwaiter().GetResult();
            Assert.IsTrue(response.IsSuccessStatusCode);

            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                var body = reader.ReadToEndAsync().Result;
                Assert.IsNotNull(body);
                Assert.IsNotEmpty(body);
            }
        }

        [Test]
        public void GetTrades_ReturnsNotEmptyTrades()
        {
            var requestUrl = "/api/TradingData/Trades";
            var response = _client.PostAsJsonAsync(requestUrl, new
            {
                PageIndex = 0,
                PageSize = 10
            }).GetAwaiter().GetResult();

            Assert.IsTrue(response.IsSuccessStatusCode);
            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                var body = reader.ReadToEndAsync().Result;
                Assert.IsNotNull(body);
                Assert.IsNotEmpty(body);
            }
        }

        [Test]
        public void GetTrades_ReturnsNotLogs()
        {
            var requestUrl = "/api/TradingData/LogRecords";
            var response = _client.PostAsJsonAsync(requestUrl, new {}).GetAwaiter().GetResult();

            Assert.IsTrue(response.IsSuccessStatusCode);
            using (var reader = new StreamReader(response.Content.ReadAsStream()))
            {
                var body = reader.ReadToEndAsync().Result;
                Assert.IsNotNull(body);
                Assert.IsNotEmpty(body);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }
    }
}