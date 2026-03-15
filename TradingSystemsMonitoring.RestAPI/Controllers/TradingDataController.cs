using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingSystemsMonitoring.Application.Abstractions;
using TradingSystemsMonitoring.Application.DTO;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.RestAPI.Metrics;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradingDataController : ControllerBase
    {
        private readonly IAccountClosedTradesService _accountClosedTradesService;
        private readonly ISecurityService _securityService;
        private readonly ITradingLogsExplorerService _tradingLogsExplorerService;

        public TradingDataController(
            IAccountClosedTradesService accountClosedTradesService,
            ISecurityService securityService,
            ITradingLogsExplorerService tradingLogsExplorerService)
        {
            _accountClosedTradesService = accountClosedTradesService;
            _securityService = securityService;
            _tradingLogsExplorerService = tradingLogsExplorerService;
        }

        /// <summary>
        /// Trade params.
        /// </summary>
        public class TradesParams
        {
            public string SystemId { get; set; }
            public long? SecurityId { get; set; }
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
            public int? PageIndex { get; set; }
            public int? PageSize { get; set; }
        }

        /// <summary>
        /// Get completed trades by params.
        /// </summary>
        /// <param name="tradesParams"><see cref="TradesParams">Trade params.</see></param>
        /// <returns>Trades collection of type <see cref="AccountClosedTradeDTO"/>.</returns>
        [AllowAnonymous]
        [HttpPost("Trades")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<AccountClosedTradeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AccountClosedTradeDTO>>> GetTrades([FromBody] TradesParams tradesParams)
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("Trades").Inc();
            if (tradesParams == null)
                return BadRequest("Request body is required.");
            var trades = await _accountClosedTradesService.SearchByParams(
                tradesParams.SystemId, tradesParams.SecurityId, tradesParams.DateFrom, tradesParams.DateTo,
                tradesParams.PageIndex, tradesParams.PageSize);
            return Ok(trades);
        }

        /// <summary>
        /// Get completed trades count by params.
        /// </summary>
        /// <param name="tradesParams"><see cref="TradesParams">Trade params.</see></param>
        /// <returns>Trades count.</returns>
        [AllowAnonymous]
        [HttpPost("TradesCount")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTradesCount([FromBody] TradesParams tradesParams)
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("TradesCount").Inc();
            if (tradesParams == null)
                return BadRequest("Request body is required.");
            var trades = await _accountClosedTradesService.CountByParams(
                tradesParams.SystemId, tradesParams.SecurityId, tradesParams.DateFrom, tradesParams.DateTo);
            return Ok(trades);
        }
        
        public class TradingLogRecordParams
        {
            public string SystemId { get; set; }
            public DateTime? Date { get; set; }
        }

        /// <summary>
        /// Get trading system log records by params.
        /// </summary>
        /// <param name="logParams"><see cref="TradingLogRecordParams">Log record params.</see></param>
        /// <returns>
        /// Trading logs collection of type <see cref="TradingLogRecordDTO"/>.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("LogRecords")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<TradingLogRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TradingLogRecordDTO>>> GetLogRecords([FromBody] TradingLogRecordParams logParams)
        {
            TsmMetrics.ApiLogRecordsRequestsTotal.Inc();
            if (logParams == null)
                return BadRequest("Request body is required.");
            var logRecords = await _tradingLogsExplorerService.GetRecordsByDate(
                logParams.SystemId, logParams.Date);
            return Ok(logRecords);
        }

        /// <summary>
        /// Get all trading system codes.
        /// </summary>
        /// <returns>System codes collection.</returns>
        [AllowAnonymous]
        [HttpGet("Trades/SystemIds")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string[]>> GetSystemIds()
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("SystemIds").Inc();
            var systemIds = await _accountClosedTradesService.GetSystemIds();
            return Ok(systemIds);
        }

        /// <summary>
        /// Get all account codes.
        /// </summary>
        /// <returns>Accounts codes collection.</returns>
        [AllowAnonymous]
        [HttpGet("Trades/AccountIds")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string[]>> GetAccountIds()
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("AccountIds").Inc();
            var systemIds = await _accountClosedTradesService.GetAccountIds();
            return Ok(systemIds);
        }

        /// <summary>
        /// Get all trading securities.
        /// </summary>
        /// <returns>Securities collection.</returns>
        [AllowAnonymous]
        [HttpGet("Securities")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Security[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Security[]>> GetSecurityIds()
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("Securities").Inc();
            var securities = await _securityService.GetSecurities();
            return Ok(securities);
        }


        /// <summary>
        /// Get completed trades by params.
        /// </summary>
        /// <param name="tradesParams"><see cref="TradesParams">Trade params.</see></param>
        /// <returns>Trades collection of type <see cref="AccountClosedTradeDTO"/>.</returns>
        [AllowAnonymous]
        [HttpGet("Trades/Current")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<AccountClosedTradeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AccountClosedTradeDTO>>> GetCurrentTrades()
        {
            TsmMetrics.ApiTradesRequestsTotal.WithLabels("Current").Inc();
            var trades = await _accountClosedTradesService.GetCurrentTradesAsync();
            return Ok(trades);
        }
    }
}
