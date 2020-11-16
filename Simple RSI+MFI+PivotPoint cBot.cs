using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
//www.jaywood.pro
//Jaywood Professional Team
//Created by : MOHAMMAD JAVAD GHANE
namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SimpleRSIcBot : Robot
    {
        [Parameter("source")]
        public DataSeries source { get; set; }

        [Parameter("period", DefaultValue = 14)]
        public int period { get; set; }

        [Parameter("lots", DefaultValue = 1)]
        public int lots { get; set; }

        [Parameter("takeProfit", DefaultValue = 150)]
        public int takeProfit { get; set; }

        [Parameter("stopLoss", DefaultValue = 0)]
        public int stopLoss { get; set; }

        private RelativeStrengthIndex rsi1;
        private MoneyFlowIndex mfi1;

        protected override void OnStart()
        {
            // Put your initialization logic here
            rsi1 = Indicators.RelativeStrengthIndex(source, period);
            mfi1 = Indicators.MoneyFlowIndex(period);
        }

        protected override void OnTick()
        {
            var Daily = MarketData.GetSeries(TimeFrame.Daily);
            var High = Daily.High.Last(1);
            var Low = Daily.Low.Last(1);
            var Close = Daily.Close.Last(1);
            var Pivot = (High + Low + Close) / 3;
            var Price = (Symbol.Ask + Symbol.Bid) / 2;
            if (Price < Pivot)
            {
                if (rsi1.Result.LastValue > 70 && mfi1.Result.LastValue > 70)
                {
                    //open sell position & close buy positions
                    openPosition(TradeType.Sell);
                    //closePosition(TradeType.Buy);
                }
            }

            if (Price > Pivot)
            {
                if (rsi1.Result.LastValue < 30 && mfi1.Result.LastValue < 30)
                {
                    // open buy position & close sell positions
                    openPosition(TradeType.Buy);
                    //closePosition(TradeType.Sell);
                }
            }

        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }

        //create the open position function
        private void openPosition(TradeType tradeType)
        {

            var positionInfo = Positions.Find("SimpleRSI-MFIcBot", Symbol, tradeType);
            //label of my bot is that name : SimpleRSIcBot
            if (positionInfo == null)
            {
                ExecuteMarketOrder(tradeType, Symbol, lots, "SimpleRSI-MFIcBot", stopLoss, takeProfit);
            }
        }

        //create the close position function
        private void closePosition(TradeType tradeType)
        {
            foreach (var positionInfo in Positions.FindAll("SimpleRSI-MFIcBot", Symbol, tradeType))
            {
                ClosePosition(positionInfo);
            }
        }
    }
}
