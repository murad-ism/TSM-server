using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;

namespace TradingSystemsMonitoring.RestAPITests
{
    public class TradingDataPublisher
    {
        private string _tradingDataReceiveUrl = "tcp://127.0.0.1:5556";
        private string _tradingDataReceiveTopic = "tradingDataChannel";

        public void Publish()
        {
            using (var publisher = new PublisherSocket())
            {
                publisher.Bind(_tradingDataReceiveUrl);
                int i = 0;
                while (i < 10)
                {
                    publisher.SendMoreFrame(_tradingDataReceiveTopic).SendFrame(i.ToString());
                    i++;
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
