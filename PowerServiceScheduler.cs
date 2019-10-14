using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PowerServiceScheduler
{
    public partial class PowerServiceScheduler : ServiceBase
    {
        EventLog _eventLog;
        Timer timer = new Timer();
        public PowerServiceScheduler()
        {
            InitializeComponent();
            Utility.EventLogger = new EventLog();
            _eventLog = Utility.EventLogger;


            if (!System.Diagnostics.EventLog.SourceExists("PowerService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "PowerService", "PowerServiceLog");
            }
            _eventLog.Source = "PowerService";
            _eventLog.Log = "PowerServiceLog";
        }

        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("Power Service Starting ");
            DateTime _currentTime = DateTime.Now;
           
            Utility.RunTrades(DateTime.Now);

            double timeInterval = 0;
            if (double.TryParse(ConfigurationManager.AppSettings["timeValue"], out timeInterval))
            {
                timer.Interval = timeInterval;
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
                _eventLog.WriteEntry(string.Format("Power Service start and first trade cycle captured at {0}", _currentTime.ToString()));
            }
            else
            {
                _eventLog.WriteEntry("timeValue from configuration could not be converted into milliseconds. Please verify");
                this.Stop();
            }
            
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            _eventLog.WriteEntry("Power service trades capturing...");
            Utility.RunTrades(DateTime.Now);            
        }

        protected override void OnStop()
        {
            _eventLog.WriteEntry("Power Service Stopping");
            if (timer != null)
                timer.Stop();
            _eventLog.WriteEntry("Power Service Stopped successfully");
        }

        
    }
}
