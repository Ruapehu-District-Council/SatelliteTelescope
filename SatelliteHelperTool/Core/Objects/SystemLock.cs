using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core.Objects
{
    public class SystemLock
    {

        public string User { get; set; }
        public string LockID { get; set; }
        public DateTime DateTime { get; set; }

        public string RequestStartedDate => DateTime.ToString("dd MMM");
        public string RequestStartedTime => DateTime.ToString("h:mmtt");

        public string Environment { get; set; }
        public int DaysOld
        {
            get
            {
                //Workout the days since the record was created.
                int Age = (int)Math.Round((DateTime.Today - DateTime).TotalDays);
                return Age > -1 ? Age : 0;
            }
        }

        public Core.Objects.SatelliteConnection SatelliteConnection { get; set; }

    }
}
