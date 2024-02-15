using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Data;
using TradingSystemsMonitoring.Data.Services;
using TradingSystemsMonitoring.DataModel.DbContext;

namespace TradingSystemsMonitoring.RestAPI.Controllers.Base
{
    public abstract class TsmDbController : ControllerBase
    {
        private readonly ILogger<TsmDbController> _logger;
        protected TsmDbService TsmDbService;
        protected TradingLogRecordDbService TradingLogRecordDbService;

        protected TsmDbController(
            ILogger<TsmDbController> logger)
        {
            _logger = logger;
            var tradingDataDbContext = new TradingDataDbContextFactory().CreateDbContext();
            TsmDbService = new TsmDbService(tradingDataDbContext);
            TradingLogRecordDbService = new TradingLogRecordDbService();
        }
    }

    public class TsmDbControllerBase : TsmDbController
    {
        protected TsmDbControllerBase(ILogger<TsmDbController> logger)
            : base(logger) { }
    }
}
