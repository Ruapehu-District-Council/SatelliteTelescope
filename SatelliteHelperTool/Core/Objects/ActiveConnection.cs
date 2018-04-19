using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core.Objects
{
    public class ActiveConnection
    {
        public string ConnectionId { get; set; }
        public string ProcessId { get; set; }
        public string ConnectionUser { get; set; }
        public string ConnectionState { get; set; }
        public DateTime ConnectionLastMonitored { get; set; }
        public string Subroutine { get; set; }
        public string RequestSessionId { get; set; }
        public string RequestSubroutine { get; set; }
        public string RequestUserId { get; set; }
        public DateTime RequestStarted { get; set; }
        public string RequestStartedDate => RequestStarted.ToString("dd MMM");
        public string RequestStartedTime => RequestStarted.ToString("h:mmtt");
        public DateTime LastRequestStarted { get; set; }
        public string LastRequestSubroutine { get; set; }
        public string Environment { get; set; }
        public int DaysOld
        {
            get
            {
                int Age = (int)Math.Round((DateTime.Today - RequestStarted).TotalDays);
                return Age > -1 ? Age : 0;
            }
        }

        public Core.Objects.SatelliteConnection SatelliteConnection { get; set; }
    }
}
