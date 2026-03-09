using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TradingSystemsMonitoring.RestAPI.Services.TradingData;

internal class KafkaConsumerHostedService : BackgroundService
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