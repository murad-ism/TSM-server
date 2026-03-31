using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using TradingSystemsMonitoring.DataModel.Entities.Kafka;

namespace TradingSystemsMonitoring.Infrastructure.Services.TradingData
{
    public class KafkaTradeConsumer : IAsyncDisposable
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

        public KafkaTradeConsumer(
            string topic,
            string dlqTopic,
            ConsumerConfig consumerConfig,
            ProducerConfig producerConfig,
            int queueCapacity,
            int workerCount,
            IConnectionMultiplexer redis,
            ILogger<KafkaTradeConsumer> logger)
        {
            if (consumerConfig == null)
                throw new ArgumentNullException(nameof(consumerConfig));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            _logger = logger;
            _topic = topic;
            _dlqTopic = dlqTopic;
            _redisDb = redis.GetDatabase();

            _consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetPartitionsAssignedHandler(OnPartitionsAssigned)
                .SetPartitionsRevokedHandler(OnPartitionsRevoked)
                .Build();

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();

            _channel = Channel.CreateBounded<ConsumeResult<string, string>>(
                new BoundedChannelOptions(queueCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

            for (int i = 0; i < workerCount; i++)
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
                    _logger.LogError(ex, "Consumer loop error");
                }
            }
        }

        private async Task WorkerLoop(CancellationToken ct)
        {
            try
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
                        // logged in processing methods
                    }
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogDebug("Kafka worker loop canceled.");
            }
        }

        private async Task ProcessMessage(
            ConsumeResult<string, string> result,
            CancellationToken ct)
        {
            for (int attempt = 1; attempt <= _maxRetryCount; attempt++)
            {
                try
                {
                    await ProcessMessageOnce(result);
                    return;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    _logger.LogDebug("Message processing canceled. Key={Key}", result.Message.Key);
                    throw;
                }
                catch (Exception ex)
                {
                    if (attempt == _maxRetryCount)
                    {
                        await HandleFinalFailure(result, ex);
                        return;
                    }

                    await DelayBeforeRetry(result, attempt, ex, ct);
                }
            }
        }

        private async Task ProcessMessageOnce(ConsumeResult<string, string> result)
        {
            var trade = JsonConvert.DeserializeObject<TradeDealResult>(result.Message.Value)
                        ?? throw new Exception("Deserialization failed");

            var saved = await SaveTrade(trade);
            if (!saved)
            {
                // Duplicate message; treat as successfully handled.
                _logger.LogDebug("Skipped duplicate trade message. Key={Key}", result.Message.Key);
            }
        }

        private async Task DelayBeforeRetry(
            ConsumeResult<string, string> result,
            int attempt,
            Exception ex,
            CancellationToken ct)
        {
            _logger.LogWarning(
                ex,
                "Message processing failed on attempt {Attempt}/{MaxRetries}. Key={Key}. Retrying in {RetryDelayMs} ms.",
                attempt,
                _maxRetryCount,
                result.Message.Key,
                _retryDelayMs);

            await Task.Delay(_retryDelayMs, ct);
        }

        private async Task HandleFinalFailure(ConsumeResult<string, string> result, Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Message processing failed after {MaxRetries} attempts, sending to DLQ. Key={Key}",
                _maxRetryCount,
                result.Message.Key);

            await SendToDlq(result, ex);
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
            _logger.LogWarning("Message sent to DLQ. Topic={DlqTopic}, Key={Key}, Error={Error}", _dlqTopic, result.Message.Key, ex.Message);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            _channel.Writer.Complete();
            try
            {
                await Task.WhenAll(_workers);
            }
            catch (OperationCanceledException)
            {
                // expected while shutting down
            }

            _consumer.Close();
            _consumer.Dispose();
            _producer.Dispose();
        }
    }
}
