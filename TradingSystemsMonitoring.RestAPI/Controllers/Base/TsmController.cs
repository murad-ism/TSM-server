using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Data.Abstractions;

namespace TradingSystemsMonitoring.RestAPI.Controllers.Base
{
    public class TsmController : ControllerBase
    {
        protected readonly ILogger<TsmController> Logger;
        protected readonly IAccountClosedTradesService AccountClosedTradesService;
        protected readonly ISecurityService SecurityService;
        protected readonly ITradingLogsExplorerService TradingLogRecordDbService;

        protected TsmController(
            ILogger<TsmController> logger,
            IAccountClosedTradesService accountClosedTradesService,
            ISecurityService securityService,
            ITradingLogsExplorerService tradingLogsExplorerService)
        {
            Logger = logger;
            AccountClosedTradesService = accountClosedTradesService;
            SecurityService = securityService;
            TradingLogRecordDbService = tradingLogsExplorerService;
        }
    }
}
