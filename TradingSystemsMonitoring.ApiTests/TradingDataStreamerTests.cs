using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetMQ;
using NetMQ.Sockets;
using TradingSystemsMonitoring.RestAPI;

namespace TradingSystemsMonitoring.ApiTests
{
    [TestFixture]
    public class TradingDataStreamerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HubConnection _connection;
        private string _received;

        [SetUp]
        public async Task Setup()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => builder
                .UseEnvironment(env));//todo:
            var server = _factory.Server.CreateHandler();
            var uri = _factory.Server.BaseAddress + "api/tradingDataMonitoring";

            _connection = new HubConnectionBuilder()
                .WithUrl(uri, options =>
                {
                    options.HttpMessageHandlerFactory = _ => server;
                })
                .Build();

            _connection.On<string>("NewTrackingData", message =>
            {
                _received = message;
            });
            await _connection.StartAsync();
        }

        [Test]
        public void SendAndReceiveMessage_Works()
        {
            var config = _factory.Services.GetRequiredService<IConfiguration>();
            using (var publisher = new PublisherSocket())
            {
                publisher.Bind(config["MsgQueueSubscriber:Url"]);
                Thread.Sleep(1000);

                for (int i = 0; i < 5; i++)
                {
                    publisher.SendMoreFrame(config["MsgQueueSubscriber:Channel"]).SendFrame("Test message_1");
                    Thread.Sleep(1);

                    publisher.SendMoreFrame(config["MsgQueueSubscriber:Channel"]).SendFrame("Test message_2");
                    Thread.Sleep(1);

                    publisher.SendMoreFrame(config["MsgQueueSubscriber:Channel"]).SendFrame("Test message_3");
                    Thread.Sleep(1);
                }

            }
            Assert.IsNotNull(_received);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _connection.DisposeAsync();
            _factory.Dispose();
        }
    }
}
