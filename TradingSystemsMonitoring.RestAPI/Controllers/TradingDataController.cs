using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradingSystemsMonitoring.Data;
using TradingSystemsMonitoring.Data.Services.DTO;
using TradingSystemsMonitoring.DataModel.Entities.Trading;
using TradingSystemsMonitoring.RestAPI.Controllers.Base;

namespace TradingSystemsMonitoring.RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradingDataController : TsmDbControllerBase
    {
        public TradingDataController(ILogger<TradingDataController> logger) : base(logger) { }

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
        /// Get completed trades entities.
        /// </summary>
        /// <param name="tradesParams"><see cref="TradesParams">Trade params.</see></param>
        /// <returns><see cref="IEnumerable{AccountClosedTradeDTO}">Trades.</see></returns>
        [AllowAnonymous]
        [HttpPost("Trades")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountClosedTradeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountClosedTradeDTO>> GetTrades([FromBody] TradesParams tradesParams)
        {
            var trades = await TsmDbService.AccountClosedTradesSvc.SearchByParams(
                tradesParams.SystemId, tradesParams.SecurityId, tradesParams.DateFrom, tradesParams.DateTo,
                tradesParams.PageIndex, tradesParams.PageSize);
            return Ok(trades);
        }

        /// <summary>
        /// Get completed trades count.
        /// </summary>
        /// <param name="tradesParams"><see cref="TradesParams">Trade params.</see></param>
        /// <returns><see cref="int">Trades count.</see></returns>
        [AllowAnonymous]
        [HttpPost("TradesCount")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> GetTradesCount([FromBody] TradesParams tradesParams)
        {
            var trades = await TsmDbService.AccountClosedTradesSvc.CountByParams(
                tradesParams.SystemId, tradesParams.SecurityId, tradesParams.DateFrom, tradesParams.DateTo);
            return Ok(trades);
        }
        
        public class TradingLogRecordParams
        {
            public string SystemId { get; set; }
            public DateTime? Date { get; set; }
        }

        /// <summary>
        /// Get log records.
        /// </summary>
        /// <param name="logParams"><see cref="TradingLogRecordParams">Log record params.</see></param>
        /// <returns><see cref="IEnumerable{TradingLogRecordDTO}">Log records.</see></returns>
        [AllowAnonymous]
        [HttpPost("LogRecords")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TradingLogRecordDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TradingLogRecordDTO>> GetLogRecords([FromBody] TradingLogRecordParams logParams)
        {
            var logRecords = await TradingLogRecordDbService.LogsSvc.GetLogRecordsByDate(
                logParams.SystemId, logParams.Date);
            return Ok(logRecords);
        }

        /// <summary>
        /// Get system ids.
        /// </summary>
        /// <returns><see cref="string[]">Securities</see></returns>
        [AllowAnonymous]
        [HttpGet("Trades/SystemIds")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string[]>> GetSystemIds()
        {
            var systemIds = await TsmDbService.AccountClosedTradesSvc.GetSystemIds();
            return Ok(systemIds);
        }

        /// <summary>
        /// Get securities.
        /// </summary>
        /// <returns><see cref="string[]">Securities</see></returns>
        [AllowAnonymous]
        [HttpGet("Trades/AccountIds")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string[]>> GetAccountIds()
        {
            var systemIds = await TsmDbService.AccountClosedTradesSvc.GetAccountIds();
            return Ok(systemIds);
        }


        /// <summary>
        /// Get securities.
        /// </summary>
        /// <returns><see cref="Security[]">Securities</see></returns>
        [AllowAnonymous]
        [HttpGet("Securities")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Security[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Security[]>> GetSecurityIds()
        {
            var securities = await TsmDbService.SecuritySvc.GetSecurities();
            return Ok(securities);
        }
    }
}
