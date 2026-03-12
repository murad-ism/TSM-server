using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NetMQ;
using NetMQ.Sockets;
using TradingSystemsMonitoring.RestAPI.Services.TradingData;
using TradingSystemsMonitoring.Tests.Helpers;

namespace TradingSystemsMonitoring.Tests
{
    [TestFixture]
    public class TradingDataSubscriberTests
    {
        private IConfiguration _config;
        private NetMqDataSubscriber _subscriber;

        [OneTimeSetUp]
        public void Setup()
        {
            _config = ConfigurationHelper.GetConfig();
        }

        [Test]
        public void TradingDataSubscriber_ReceivePackets_LossIsLessThan1Percent()
        {
            const int packetsCount = 1000;
            var counter = 0;
            var logger = new Mock<ILogger<NetMqDataSubscriber>>().Object;
            _subscriber = new NetMqDataSubscriber(_config, logger);
            _subscriber.OnDataReceived += _ => counter++;
            _subscriber.Subscribe(CancellationToken.None);
            Thread.Sleep(1);

            using (var publisher = new PublisherSocket())
            {
                publisher.Bind(_config["MsgQueueSubscriber:Url"]);
                Thread.Sleep(1000);

                var i = 0;
                while (i++ < packetsCount)
                {
                    publisher.SendMoreFrame(_config["MsgQueueSubscriber:Channel"]).SendFrame("Test message");
                    Thread.Sleep(1);
                }
            }
            
            Assert.IsTrue(counter > 0);
            Assert.IsTrue(counter > packetsCount * 0.99);
        }

        [OneTimeTearDown]
        public void Clean()
        {
        }
    }
}