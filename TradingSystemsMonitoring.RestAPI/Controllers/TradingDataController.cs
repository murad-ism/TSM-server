using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Data.Abstractions;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.RestAPI.Controllers.Base;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradingDataController : TsmController
    {
        public TradingDataController(
            ILogger<TradingDataController> logger,
            IAccountClosedTradesService accountClosedTradesService,
            ISecurityService securityService,
            ITradingLogsExplorerService tradingLogsExplorerService)
            : base(logger, accountClosedTradesService, securityService, tradingLogsExplorerService)
        {
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
            if (tradesParams == null)
                return BadRequest("Request body is required.");
            var trades = await AccountClosedTradesService.SearchByParams(
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
            if (tradesParams == null)
                return BadRequest("Request body is required.");
            var trades = await AccountClosedTradesService.CountByParams(
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
            if (logParams == null)
                return BadRequest("Request body is required.");
            var logRecords = await TradingLogRecordDbService.GetRecordsByDate(
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
            var systemIds = await AccountClosedTradesService.GetSystemIds();
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
            var systemIds = await AccountClosedTradesService.GetAccountIds();
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
            var securities = await SecurityService.GetSecurities();
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
            var trades = await AccountClosedTradesService.GetCurrentTradesAsync();
            return Ok(trades);
        }
    }
}
