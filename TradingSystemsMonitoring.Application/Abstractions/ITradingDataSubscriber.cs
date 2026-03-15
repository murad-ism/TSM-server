using System;
using System.Threading;
using System.Threading.Tasks;

namespace TradingSystemsMonitoring.Application.Abstractions
{
    public interface ITradingDataSubscriber
    {
        event Action<string> OnDataReceived;
        Task Subscribe(CancellationToken token);
    }
}
