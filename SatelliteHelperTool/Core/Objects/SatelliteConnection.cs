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

        //Connect to the Satellite
        public void Connect()
        {
            try
            {
                //Get a new Session ID for the connection
                SessionID = SatelliteUtil.NewSessionId();
                //Get the username for the current user to pass to Satellite
                string UserID = Environment.UserName;

                //Get the Sattelite object, i think this is WCF stuff
                satelliteManager = (ISatelliteManager)Activator.GetObject(typeof(ISatelliteManager), SaterliteURL);

                //Register on the Satellite service
                SatelliteUtil.RegisterSatellite(satelliteManager, SessionID, UserID, "Satellite.Telescope", Port.ToString(), Environment.MachineName);
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
                //Remove the connction for the Satellite.
                satelliteManager.RemoveClient(SessionID);
                Connected = false;
            }
        }

    }
}
