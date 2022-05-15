using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradingSystemsMonitoring.RestAPI.Hubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingSystemsMonitoring.RestAPI.Services.TrackingDataReceiver;

namespace TradingSystemsMonitoring.RestAPI.Hubs.Tests
{
    [TestClass()]
    public class TradingDataMontioringHubTests
    {
        [TestMethod()]
        public void TradingDataMontioringHubTest()
        {
            var hub = new TradingDataMonitoringHub();
            //hub.GetTrackingTradingData();
        }

        [TestMethod()]
        public void GetTradingDataTest()
        {
            TrackingDataReceiver receiver = new TrackingDataReceiver();
            

            //var cts = new CancellationTokenSource();
            //Test(cts.Token);
            while (true)
            {
                
            }
        }

        public async Task Test(CancellationToken token)
        {
            while (true)
            {
                await Task.Delay(5000, token);
            }
        }
    }
}