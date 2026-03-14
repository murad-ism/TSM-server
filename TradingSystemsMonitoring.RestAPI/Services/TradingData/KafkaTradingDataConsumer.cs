using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using TradingSystemsMonitoring.DataModel.Entities.Kafka;
using TradingSystemsMonitoring.RestAPI.Metrics;
using TradingSystemsMonitoring.RestAPI.Settings;

namespace TradingSystemsMonitoring.RestAPI.Services.TradingData
{
    internal class KafkaTradeConsumer : IAsyncDisposable
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IProducer<string, string> _producer;
        private readonly IDatabase _redisDb;
        private readonly ILogger<KafkaTradeConsumer> _logger;
        private readonly string _topic;
        private readonly string _dlqTopic;

        private readonly Channel<ConsumeResult<string, string>> _channel;
        private readonly CancellationTokenSource _cts = new();
        private readonly List<Task> _workers = new();

        private readonly int _maxRetryCount = 3;
        private readonly int _retryDelayMs = 1000;

        public KafkaTradeConsumer(KafkaSettings kafkaSettings, IConnectionMultiplexer redis, ILogger<KafkaTradeConsumer> logger)
        {
            if (kafkaSettings == null)
                throw new ArgumentNullException(nameof(kafkaSettings));

            _logger = logger;
            _topic = kafkaSettings.Topic;
            _dlqTopic = kafkaSettings.DlqTopic;
            _redisDb = redis.GetDatabase();

            var consumerConfig = kafkaSettings.CreateConsumerConfig();
            var producerConfig = kafkaSettings.CreateProducerConfig();

            _consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetPartitionsAssignedHandler(OnPartitionsAssigned)
                .SetPartitionsRevokedHandler(OnPartitionsRevoked)
                .Build();

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();

            _channel = Channel.CreateBounded<ConsumeResult<string, string>>(
                new BoundedChannelOptions(kafkaSettings.QueueCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

            for (int i = 0; i < kafkaSettings.WorkerCount; i++)
                _workers.Add(Task.Run(() => WorkerLoop(_cts.Token)));
        }

        private void OnPartitionsAssigned(IConsumer<string, string> consumer, List<TopicPartition> partitions)
        {
            _logger.LogInformation("Partitions assigned: {Partitions}", string.Join(",", partitions));
            foreach (var tp in partitions)
            {
                _logger.LogDebug("Assigned partition {Partition} of topic {Topic}", tp.Partition.Value, tp.Topic);
            }
        }

        private void OnPartitionsRevoked(IConsumer<string, string> consumer, List<TopicPartitionOffset> partitions)
        {
            _logger.LogInformation("Partitions revoked: {Partitions}", string.Join(",", partitions));
            try
            {
                consumer.Commit();
                _logger.LogDebug("Offsets committed before partition revocation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing offsets during rebalance");
            }
        }

        public Task StartAsync()
        {
            _consumer.Subscribe(_topic);
            return Task.Run(() => ConsumeLoop(_cts.Token));
        }

        private async Task ConsumeLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(ct);
                    await _channel.Writer.WriteAsync(result, ct);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    TsmMetrics.KafkaConsumerErrorsTotal.WithLabels(_topic, "consume").Inc();
                    _logger.LogError(ex, "Consumer loop error");
                }
            }
        }

        private async Task WorkerLoop(CancellationToken ct)
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    await ProcessMessage(message, ct);
                    _consumer.Commit(message);
                }
                catch (Exception)
                {
                    TsmMetrics.KafkaConsumerErrorsTotal.WithLabels(_topic, "process").Inc();
                }
            }
        }

        private async Task ProcessMessage(
            ConsumeResult<string, string> result,
            CancellationToken ct)
        {
            var stopwatch = Stopwatch.StartNew();
            int attempt = 0;

            while (attempt < _maxRetryCount)
            {
                try
                {
                    var trade = JsonConvert.DeserializeObject<TradeDealResult>(result.Message.Value)
                                ?? throw new Exception("Deserialization failed");

                    var saved = await SaveTrade(trade);

                    if (!saved)
                    {
                        TsmMetrics.KafkaMessagesProcessedTotal.WithLabels(_topic, "duplicate").Inc();
                        TsmMetrics.KafkaProcessingDurationSeconds.WithLabels(_topic).Observe(stopwatch.Elapsed.TotalSeconds);
                        return; // duplicate
                    }

                    TsmMetrics.KafkaMessagesProcessedTotal.WithLabels(_topic, "success").Inc();
                    TsmMetrics.KafkaProcessingDurationSeconds.WithLabels(_topic).Observe(stopwatch.Elapsed.TotalSeconds);
                    return;
                }
                catch (Exception ex)
                {
                    attempt++;
                    if (attempt >= _maxRetryCount)
                    {
                        _logger.LogWarning(ex, "Message processing failed after {MaxRetries} attempts, sending to DLQ. Key={Key}", _maxRetryCount, result.Message.Key);
                        await SendToDlq(result, ex);
                        TsmMetrics.KafkaMessagesProcessedTotal.WithLabels(_topic, "dlq").Inc();
                        TsmMetrics.KafkaProcessingDurationSeconds.WithLabels(_topic).Observe(stopwatch.Elapsed.TotalSeconds);
                    }
                    else
                        await Task.Delay(_retryDelayMs, ct);
                }
            }
        }

        private async Task<bool> SaveTrade(TradeDealResult trade)
        {
            var processedKey = $"processed:{trade.Id}";
            var tradeKey = $"trade:{trade.Id}";
            var indexKey = "trades:index";

            var json = JsonConvert.SerializeObject(trade);
            var tran = _redisDb.CreateTransaction();

            tran.AddCondition(Condition.KeyNotExists(processedKey));

            _ = tran.StringSetAsync(processedKey, "1", TimeSpan.FromDays(1));
            _ = tran.StringSetAsync(tradeKey, json);
            _ = tran.SetAddAsync(indexKey, trade.Id.ToString());

            return await tran.ExecuteAsync();
        }

        private async Task SendToDlq(ConsumeResult<string, string> result, Exception ex)
        {
            var dlqMessage = new
            {
                result.Message.Key,
                result.Message.Value,
                error = ex.Message,
                time = DateTime.UtcNow
            };

            await _producer.ProduceAsync(
                _dlqTopic,
                new Message<string, string>
                {
                    Key = result.Message.Key,
                    Value = JsonConvert.SerializeObject(dlqMessage)
                });
            TsmMetrics.KafkaDlqSentTotal.WithLabels(_topic, _dlqTopic).Inc();
            _logger.LogWarning("Message sent to DLQ. Topic={DlqTopic}, Key={Key}, Error={Error}", _dlqTopic, result.Message.Key, ex.Message);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _channel.Writer.Complete();
            await Task.WhenAll(_workers);

            _consumer.Close();
            _consumer.Dispose();
            _producer.Dispose();
        }
    }
}
