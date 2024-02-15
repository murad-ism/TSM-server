using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;

namespace TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver
{
    public interface ITrackingDataReceiver
    {
        public event Action<string> OnDataReceived;
        public Task BeginReceiveData(CancellationToken token);
    }

    public class TrackingDataReceiver : ITrackingDataReceiver
    {
        //todo: !!! вынести в конфиг
        private string _tradingDataReceiveUrl = "tcp://127.0.0.1:5556";
        private string _tradingDataReceiveTopic = "tradingDataChannel";
        public event Action<string> OnDataReceived;
        private ILogger<TrackingDataReceiver> _logger;
        public TrackingDataReceiver(ILogger<TrackingDataReceiver> logger)
        {
            _logger = logger;
        }

        public async Task BeginReceiveData(CancellationToken token)
        {
            await Task.Run(ReceiveData);
        }
        
        private void ReceiveData()
        {
            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect(_tradingDataReceiveUrl);
                subscriber.Subscribe(_tradingDataReceiveTopic);

                while (true)
                {
                    var topic = subscriber.ReceiveFrameString();
                    var msg = subscriber.ReceiveFrameString();
                    OnDataReceived?.Invoke(msg);
                }
            }
        }
    }
}
