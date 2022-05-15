using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver
{
    public interface ITrackingDataReceiver
    {
        public event Action<string> DataReceived;
        public void StartDataReceive();
    }

    public class TrackingDataReceiver : ITrackingDataReceiver
    {
        public event Action<string> DataReceived;
        private string _hostName = "localhost";
        private string _queueId = "algoTradeTrackingDataQueue";

        public void StartDataReceive()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueId,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                DataReceived(message);
            };
            channel.BasicConsume(queue: _queueId,
                autoAck: true,
                consumer: consumer);
        }
    }
}
