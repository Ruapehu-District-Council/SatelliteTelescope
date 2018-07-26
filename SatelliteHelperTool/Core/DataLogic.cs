using Origen.Satellite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SatelliteHelperTool.Core
{
    public class DataLogic
    {
        public bool RunningAsCLI { get; set; } = false;

        //Connect to the Satellites
        public void Connect(List<Objects.SatelliteConnection> SatelliteConnection)
        {
            SatelliteConnection.ForEach(Connection =>
            {
                Connection.Connect();
            });
        }

        //Disconnect from the Satellites
        public void Disconnect(List<Objects.SatelliteConnection> SatelliteConnection)
        {
            SatelliteConnection.ForEach(Connection =>
            {
                Connection.Disconnect();
            });
        }

        //Get the Active Connections for all Satellites
        public List<Objects.ActiveConnection> GetActiveConnections(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Objects.ActiveConnection> ActiveConnections = new List<Objects.ActiveConnection>();

            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    //Get the name of the Environment where the connetion is
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    SatelliteConnection[] connList = ConnectionDetails.GetSatelliteManager().ConnectionList;
                    if (connList != null)
                    {
                        //Go over all the connections
                        foreach (SatelliteConnection conn2 in connList)
                        {
                            //If the connection has been marked as kill, continue
                            if (conn2.ConnectionState == SatelliteConnectionState.Kill)
                            {
                                continue;
                            }

                            //See if there was any subroutine aka function that the user was doing 
                            string subroutine = string.Empty;
                            if (conn2.Request.Data.ContainsKey("subroutine"))
                            {
                                subroutine = conn2.Request.Data["subroutine"].ToString();
                            }

                            //Remap to my new object
                            Objects.ActiveConnection ActiveConnection = new Objects.ActiveConnection()
                            {
                                ConnectionId = conn2.ConnectionId,
                                ConnectionLastMonitored = conn2.ConnectionLastMonitored,
                                ConnectionState = conn2.ConnectionState.ToString(),
                                ConnectionUser = conn2.ConnectionUser,
                                LastRequestStarted = conn2.LastRequestStarted,
                                LastRequestSubroutine = conn2.LastRequestSubroutine,
                                ProcessId = conn2.ProcessId,
                                RequestSessionId = conn2.RequestSessionId,
                                RequestStarted = conn2.RequestStarted,
                                RequestSubroutine = conn2.RequestSubroutine,
                                RequestUserId = conn2.RequestUserId,
                                Subroutine = subroutine,
                                Environment = Environment,
                                SatelliteConnection = ConnectionDetails
                            };

                            ActiveConnections.Add(ActiveConnection);
                        }
                    }
                }
                catch (Exception e)
                {
                    
                }
            });

            return ActiveConnections;
        }

        //Get the Clients for all Satellites
        public List<Core.Objects.Client> GetClients(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Core.Objects.Client> Clients = new List<Objects.Client>();
            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    //Get the name of the Environment where the connetion is
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    SatelliteClient[] SC = ConnectionDetails.GetSatelliteManager().ClientList;
                    foreach (SatelliteClient Client in SC)
                    {
                        //Remap to my new object
                        Clients.Add(new Core.Objects.Client
                        {
                            HostIP = Client.HostIP,
                            UserId = Client.UserId,
                            HostName = Client.HostName,
                            LastActivity = Client.LastActivity,
                            SesstionID = Client.SessionId,
                            Environment = Environment,
                            SatelliteConnection = ConnectionDetails

                        });
                    }
                }
                catch
                {

                }
            });

            return Clients;
        }

        //Get the locks for all Satellites 
        public List<Objects.SystemLock> GetLocks(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Objects.SystemLock> SystemLocks = new List<Objects.SystemLock>();

            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    //Get the name of the Environment where the connetion is
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    List<LockObject> lockObjects = ConnectionDetails.GetSatelliteManager().GetCurrentLocks();
                    foreach(LockObject Lock in lockObjects)
                    {
                        //Remap to my new object
                        SystemLocks.Add(new Objects.SystemLock()
                        {
                            User = Lock.User,
                            LockID = Lock.LockId,
                            DateTime = Lock.DateTime,
                            Environment = Environment,
                            SatelliteConnection = ConnectionDetails

                        });
                    }
                }
                catch
                {

                }
            });

            return SystemLocks;
        }       

        //Remove a Client from it's parrent Satellite
        public void RemoveClient(Core.Objects.Client Client, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            Objects.ActiveConnection activeConnection = GetActiveConnections(SatelliteConnectionDetails).First(Connection => Connection.RequestSessionId == Client.SesstionID);
            //Look to makesure there isn't a Connection for that Client. If there is don't remove the client.
            if (activeConnection == null)
            {
                Client.SatelliteConnection.GetSatelliteManager().RemoveClient(Client.SesstionID);
            }
            else
            {
                //Only display message if not running with CLI arguments
                if (!RunningAsCLI)
                {
                    MessageBox.Show("Unable to remove Client: " + Client.HostName + " as they have a Connection still processing: " + activeConnection.RequestSubroutine + " " + activeConnection.Subroutine);
                }
            }
        }

        //Remove a Connection from it's parrent Satellite
        public void RemoveConnection(Core.Objects.ActiveConnection ActiveConnection)
        {
            ActiveConnection.SatelliteConnection.GetSatelliteManager().RemoveClient(ActiveConnection.RequestSessionId);
        }

        //Remove all clients that are older than days given accross all Satellites
        public void RemoveClientsOlderThan(int Days, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetClients(SatelliteConnectionDetails).Where(Client => Client.DaysOld > Days).ToList().ForEach(Client =>
            {
                RemoveClient(Client, SatelliteConnectionDetails);
            });
        }

        //Remove all clients where the user is the one given accross all Satellites
        public void RemoveClientsByUserID(string UserID, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetClients(SatelliteConnectionDetails).Where(Client => Client.UserId == UserID).ToList().ForEach(Client =>
            {
                RemoveClient(Client, SatelliteConnectionDetails);
            });
        }

        //Remove all connections that are older than days given accross all Satellites
        public void RemoveConnectionsOldetThan(int Days, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetActiveConnections(SatelliteConnectionDetails).Where(Connection => Connection.DaysOld > Days).ToList().ForEach(Connection =>
            {
                RemoveConnection(Connection);
            });
        }

        //Remove all connections where the user is the one given accross all Satellites
        public void RemoveConnectionByUserID(string UserID, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetActiveConnections(SatelliteConnectionDetails).Where(Connection => Connection.ConnectionUser == UserID).ToList().ForEach(Connection =>
            {
                RemoveConnection(Connection);
            });
        }
    }
}
