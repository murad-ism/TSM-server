using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;
using TradingSystemsMonitoring.DataModel.Entities.Kafka;
using TradingSystemsMonitoring.DataModel.Entities.Trading;

public class KafkaTradeConsumer : IAsyncDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IProducer<string, string> _producer;
    private readonly IDatabase _redisDb;

    private readonly string _topic;
    private readonly string _dlqTopic;

    private readonly Channel<ConsumeResult<string, string>> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly List<Task> _workers = new();

    private readonly int _maxRetryCount = 3;
    private readonly int _retryDelayMs = 1000;

    public KafkaTradeConsumer(
        ConsumerConfig consumerConfig,
        ProducerConfig producerConfig,
        ConnectionMultiplexer redis,
        string topic,
        string dlqTopic,
        int workerCount = 4,
        int queueCapacity = 10000)
    {
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
        Console.WriteLine($"[Rebalance] Partitions assigned: {string.Join(",", partitions)}");
        foreach (var tp in partitions)
        {
            Console.WriteLine($"Assigned partition {tp.Partition.Value} of topic {tp.Topic}");
        }
    }

    private void OnPartitionsRevoked(IConsumer<string, string> consumer, List<TopicPartitionOffset> partitions)
    {
        Console.WriteLine($"[Rebalance] Partitions revoked: {string.Join(",", partitions)}");
        try
        {
            consumer.Commit();
            Console.WriteLine("Offsets committed before partition revocation");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error committing offsets during rebalance: {ex.Message}");
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
                Console.WriteLine($"Consumer loop error: {ex.Message}");
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
            catch { /* Kafka retry */ }
        }
    }

    private async Task ProcessMessage(
        ConsumeResult<string, string> result,
        CancellationToken ct)
    {
        int attempt = 0;

        while (attempt < _maxRetryCount)
        {
            try
            {
                var trade = JsonConvert.DeserializeObject<TradeDealResult>(result.Message.Value)
                            ?? throw new Exception("Deserialization failed");

                var saved = await SaveTrade(trade);

                if (!saved)
                    return; // duplicate

                return;
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt >= _maxRetryCount)
                    await SendToDlq(result, ex);
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

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaTradeConsumer _consumer;

    public KafkaConsumerHostedService(KafkaTradeConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.StartAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        await _consumer.DisposeAsync();
    }
}

