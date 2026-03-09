using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace TradingSystemsMonitoring.RestAPI.Settings
{
    /// <summary>
    /// Kafka configuration for the trade consumer (bootstrap, topic, DLQ, worker count, queue capacity).
    /// </summary>
    public class KafkaSettings
    {
        public string BootstrapServers { get; private set; }
        public string GroupId { get; private set; }
        public string Topic { get; private set; }
        public string DlqTopic { get; private set; }
        public int WorkerCount { get; private set; }
        public int QueueCapacity { get; private set; }

        public static KafkaSettings Instance { get; private set; }

        public static void ReadConfiguration(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Instance = new KafkaSettings
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
                GroupId = configuration["Kafka:GroupId"] ?? "trade-consumer-group",
                Topic = configuration["Kafka:Topic"] ?? "trade-deals-topic",
                DlqTopic = configuration["Kafka:DlqTopic"] ?? "trade-deals-dlq",
                WorkerCount = configuration.GetValue("Kafka:WorkerCount", 4),
                QueueCapacity = configuration.GetValue("Kafka:QueueCapacity", 10000)
            };
        }

        public ConsumerConfig CreateConsumerConfig()
        {
            return new ConsumerConfig
            {
                BootstrapServers = BootstrapServers,
                GroupId = GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
        }

        public ProducerConfig CreateProducerConfig()
        {
            return new ProducerConfig
            {
                BootstrapServers = BootstrapServers
            };
        }
    }
}
