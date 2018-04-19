using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core.Objects
{
    public class Client
    {
        public string Environment { get; set; }
        public string HostIP { get; set; }
        public string HostName { get; set; }
        public string UserId { get; set; }
        public string SesstionID { get; set; }
        public DateTime LastActivity { get; set; }
        public int DaysOld
        {
            get
            {
                int Age = (int)Math.Round((DateTime.Today - LastActivity).TotalDays);
                return Age > -1  ? Age : 0;
            }
        }

        public string StartDate => LastActivity.ToString("dd MMM");
        public string StartTime => LastActivity.ToString("h:mmtt");

        public Core.Objects.SatelliteConnection SatelliteConnection { get; set; }

    }
}
