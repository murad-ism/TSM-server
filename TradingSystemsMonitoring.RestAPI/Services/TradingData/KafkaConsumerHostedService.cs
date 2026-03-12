using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TradingSystemsMonitoring.RestAPI.Services.TradingData;

internal class KafkaConsumerHostedService : BackgroundService
{
    private readonly KafkaTradeConsumer _consumer;
    private readonly ILogger<KafkaConsumerHostedService> _logger;

    public KafkaConsumerHostedService(KafkaTradeConsumer consumer, ILogger<KafkaConsumerHostedService> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer started.");
        await _consumer.StartAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka consumer stopping.");
        await base.StopAsync(cancellationToken);
        await _consumer.DisposeAsync();
    }
}