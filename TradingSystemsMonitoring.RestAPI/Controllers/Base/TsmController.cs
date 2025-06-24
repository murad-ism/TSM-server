using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Data.Services;
using TradingSystemsMonitoring.DataModel.DbContext;
using TradingSystemsMonitoring.DataModel.DbContext.Factories;

namespace TradingSystemsMonitoring.RestAPI.Controllers.Base
{
    public class TsmController : ControllerBase
    {
        private readonly ILogger<TsmController> _logger;
        protected AccountClosedTradesService AccountClosedTradesService;
        protected SecurityService SecurityService;
        protected TradingLogsExplorerService TradingLogRecordDbService;
        protected TsmController(ILogger<TsmController> logger, IDbContextFactory<TradingDataDbContext> dbFactory, 
            ITradingLogRecordDbFactory tradingLogRecordDbFactory)
        {
            _logger = logger;
            AccountClosedTradesService = new AccountClosedTradesService(dbFactory);
            SecurityService = new SecurityService(dbFactory);
            TradingLogRecordDbService = new TradingLogsExplorerService(tradingLogRecordDbFactory);
        }
    }
}
