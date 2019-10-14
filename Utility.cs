using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerServiceScheduler
{
    /// <summary>
    /// Utility class for all control methods 
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Event logger instance for the whole application
        /// </summary>
        public static EventLog EventLogger { get; set; }

        /// <summary>
        /// Fetch trades and generate CSV file for captured trades
        /// </summary>
        /// <param name="pTradeDate"></param>
        public static void RunTrades(DateTime pTradeDate)
        {
            try
            {
                string FilePath = string.Format("{0}PowerPosition_{1}.csv", ConfigurationManager.AppSettings["csvFilePath"], pTradeDate.ToString("yyyyMMdd_HHmm"));
                PowerService _powerService = new PowerService();

                List<PowerTrade> powerTrades = new List<PowerTrade>();
                powerTrades.AddRange(_powerService.GetTrades(pTradeDate));

                PowerTrade result = PowerTrade.Create(pTradeDate, 24);// creating 24 power period for 24 hrs trades

                foreach (var item in powerTrades)
                {
                    foreach (PowerPeriod power in item.Periods)
                    {
                        var pow = result.Periods.FirstOrDefault(x => x.Period == power.Period);
                        if (pow != null)
                            pow.Volume += power.Volume;
                    }
                }


                //Sample code for verifying the total result
                int _counter = 1;
                foreach(PowerTrade p in powerTrades)
                {
                    ExportTradeToCSV(p, FilePath.Replace(".csv",string.Format("_{0}.csv",_counter)), false);
                    _counter++;

                }
                

                
                
                ExportTradeToCSV(result, FilePath, false);
            }
            catch(Exception ex)
            {
                EventLogger.WriteEntry(string.Format("Trade capturing failed at {0}.\n Failure Reason : {1}", pTradeDate.ToString(), ex.Message));
            }
        }

        private static void ExportTradeToCSV(PowerTrade pTrade, string pCSVFilePath, bool pAppend)
        {
            if (!string.IsNullOrEmpty(pCSVFilePath))
            {
                
                using (CsvWriter writer = new CsvWriter(pCSVFilePath, pAppend,','))
                {
                    if (!pAppend)
                    {
                        CsvRow row = new CsvRow();
                        row.Add("LocalTime");
                        row.Add("Volume");

                        writer.WriteRow(row);
                    }

                    foreach (PowerPeriod period in pTrade.Periods)
                    {
                        CsvRow row = new CsvRow();
                        row.Add(ConvertPeriodToTime(period.Period));
                        row.Add(period.Volume);
                        writer.WriteRow(row);
                    }
                }
            }
            else
                throw new Exception("CSV file path was empty");
        }

        private static string ConvertPeriodToTime(int period)
        {
            int _hour;
            if (period == 1)
                _hour = 23;
            else
                _hour = period - 2;


            DateTime _result = new DateTime(1, 1, 1, _hour, 0, 0);
            return _result.ToString("HH:mm");
        }
    }
}
