using Microsoft.AspNetCore.SignalR;
using Moq;
using TradingSystemsMonitoring.RestAPI.Hubs;
using TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver;
using TradingSystemsMonitoring.RestAPI.Services.TradingData;

namespace TradingSystemsMonitoring.Tests
{
    public class TradingDataMonitoringHubTests
    {
        private Mock<IHubContext<TradingDataMonitoringHub>> _hubContextMock;
        private Mock<IHubClients> _clientsMock;
        private Mock<IClientProxy> _clientProxyMock;
        private TradingDataStreamer _service;

        [OneTimeSetUp]
        public void Setup()
        {
            _hubContextMock = new Mock<IHubContext<TradingDataMonitoringHub>>();
            _clientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            _hubContextMock.Setup(h => h.Clients).Returns(_clientsMock.Object);
            _clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
            _service = new TradingDataStreamer(null, null, _hubContextMock.Object);
        }

        [Test]
        public void NotifyAllAsync_SendsMessage()
        {
            var message = "Hello test";
            _service.TradingDataSubscriberDataReceived(message);
            Assert.DoesNotThrow(() => 
                _clientProxyMock.Verify(
                    c => c.SendCoreAsync("NewTrackingData", It.Is<object[]>(o => (string)o[0] == message),
                    default)
                    , Times.Once));
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            _service.Dispose();
        }
    }
}