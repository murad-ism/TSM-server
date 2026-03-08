using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.Configuration;
using NetMQ;
using NetMQ.Sockets;

namespace TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver
{
    public interface ITradingDataSubscriber
    {
        public event Action<string> OnDataReceived;
        public Task Subscribe(CancellationToken token);
    }

    public class NetMqDataSubscriber : ITradingDataSubscriber
    {
        private IConfiguration _config;
        public event Action<string> OnDataReceived;
        public NetMqDataSubscriber(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task Subscribe(CancellationToken token)
        {
            await Task.Run(ReceiveData, token);
        }
        
        private void ReceiveData()
        {
            var url = _config["MsgQueueSubscriber:Url"];
            var channel = _config["MsgQueueSubscriber:Channel"];

            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidConfigurationException("Param 'MsgQueueSubscriber:Url' is null");
            }

            if (string.IsNullOrEmpty(channel))
            {
                throw new InvalidConfigurationException("Param 'MsgQueueSubscriber:Channel' is null");
            }

            using (var subscriber = new SubscriberSocket())
            {
                subscriber.Connect(url);
                subscriber.Subscribe(channel);

                if (OnDataReceived != null)
                {
                    while (true)
                    {
                        var topic = subscriber.ReceiveFrameString();
                        var msg = subscriber.ReceiveFrameString();
                        OnDataReceived(msg);
                    }
                }

                //if (OnDataReceived != null)
                //{
                //    using (var poller = new NetMQPoller { subscriber })
                //    {
                //        subscriber.ReceiveReady += (s, e) =>
                //        {
                //            string message = e.Socket.ReceiveFrameString();
                //            OnDataReceived(message);
                //        };

                //        poller.Run(); // Starts the event-driven loop
                //    }
                //}

            }
        }
    }
}
