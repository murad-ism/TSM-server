using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NetMQ;
using NetMQ.Sockets;
using TradingSystemsMonitoring.RestAPI.Abstractions;

namespace TradingSystemsMonitoring.RestAPI.Services.TradingData
{
    public class NetMqDataSubscriber : ITradingDataSubscriber
    {
        private IConfiguration _config;
        public event Action<string> OnDataReceived;
        public NetMqDataSubscriber(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Subscribes to the message queue and receives messages until cancellation is requested.
        /// </summary>
        public async Task Subscribe(CancellationToken token)
        {
            await Task.Run(() => ReceiveData(token), token);
        }

        private void ReceiveData(CancellationToken token)
        {
            var url = _config["MsgQueueSubscriber:Url"];
            var channel = _config["MsgQueueSubscriber:Channel"];

            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("Param 'MsgQueueSubscriber:Url' is null or empty in configuration.");
            }

            if (string.IsNullOrEmpty(channel))
            {
                throw new InvalidOperationException("Param 'MsgQueueSubscriber:Channel' is null or empty in configuration.");
            }

            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect(url);
                subscriber.Subscribe(channel);

                if (OnDataReceived != null)
                {
                    var timeout = TimeSpan.FromMilliseconds(500);
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            if (subscriber.TryReceiveFrameString(timeout, out _) && subscriber.TryReceiveFrameString(timeout, out string msg))
                            {
                                OnDataReceived(msg);
                            }
                        }
                        catch (Exception) when (token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
