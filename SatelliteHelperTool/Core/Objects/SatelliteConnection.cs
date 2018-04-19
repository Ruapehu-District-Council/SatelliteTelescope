using AutoFormGenorator.Object;
using Origen.Satellite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core.Objects
{
    [FormClass(FormValueWidth = 500)]
    public class SatelliteConnection
    {

        private string SessionID;
        private ISatelliteManager satelliteManager;

        [FormField(Required = true)]
        public string SaterliteURL { get; set; }

        [FormField(Required = true)]
        public int Port { get; set; }

        public bool Connected { get; set; }

        public void Connect()
        {
            try
            {
                SessionID = SatelliteUtil.NewSessionId();
                string UserID = Environment.UserName;

                satelliteManager = (ISatelliteManager)Activator.GetObject(typeof(ISatelliteManager), SaterliteURL);

                SatelliteUtil.RegisterSatellite(satelliteManager, SessionID, UserID, "HelperTool", Port.ToString(), Environment.MachineName);
                Connected = true;
            }
            catch
            {

            }
        }

        public ISatelliteManager GetSatelliteManager()
        {
            return satelliteManager;
        }

        public void Disconnect()
        {
            if (SessionID != null)
            {
                satelliteManager.RemoveClient(SessionID);
            }
        }

    }
}
