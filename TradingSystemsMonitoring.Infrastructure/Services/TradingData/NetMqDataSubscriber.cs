using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using TradingSystemsMonitoring.Application.Abstractions;

namespace TradingSystemsMonitoring.Infrastructure.Services.TradingData
{
    public class NetMqDataSubscriber : ITradingDataSubscriber
    {
        private readonly IConfiguration _config;
        private readonly ILogger<NetMqDataSubscriber> _logger;

        public event Action<string> OnDataReceived;

        public NetMqDataSubscriber(IConfiguration config, ILogger<NetMqDataSubscriber> logger)
        {
            _config = config;
            _logger = logger;
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
                _logger.LogWarning("Param 'MsgQueueSubscriber:Url' is null or empty in configuration.");
                throw new InvalidOperationException("Param 'MsgQueueSubscriber:Url' is null or empty in configuration.");
            }

            if (string.IsNullOrEmpty(channel))
            {
                _logger.LogWarning("Param 'MsgQueueSubscriber:Channel' is null or empty in configuration.");
                throw new InvalidOperationException("Param 'MsgQueueSubscriber:Channel' is null or empty in configuration.");
            }

            _logger.LogInformation("Subscribing to NetMQ. Url={Url}, Channel={Channel}", url, channel);

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
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error receiving data from NetMQ. Url={Url}, Channel={Channel}", url, channel);
                            break;
                        }
                    }
                }
            }
        }
    }
}
