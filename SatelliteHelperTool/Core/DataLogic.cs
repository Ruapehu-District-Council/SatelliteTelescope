using Origen.Satellite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core
{
    public class DataLogic
    {

        public void Connect(List<Objects.SatelliteConnection> SatelliteConnection)
        {
            SatelliteConnection.ForEach(Connection =>
            {
                Connection.Connect();
            });
        }

        public void Disconnect(List<Objects.SatelliteConnection> SatelliteConnection)
        {
            SatelliteConnection.ForEach(Connection =>
            {
                Connection.Disconnect();
            });
        }

        public List<Objects.ActiveConnection> GetActiveConnections(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Objects.ActiveConnection> ActiveConnections = new List<Objects.ActiveConnection>();

            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    SatelliteConnection[] connList = ConnectionDetails.GetSatelliteManager().ConnectionList;
                    if (connList != null)
                    {
                        foreach (SatelliteConnection conn2 in connList)
                        {
                            if (conn2.ConnectionState == SatelliteConnectionState.Kill)
                            {
                                continue;
                            }

                            string subroutine = string.Empty;
                            if (conn2.Request.Data.ContainsKey("subroutine"))
                            {
                                subroutine = conn2.Request.Data["subroutine"].ToString();
                            }

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

        public List<Core.Objects.Client> GetClients(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Core.Objects.Client> Clients = new List<Objects.Client>();
            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    SatelliteClient[] SC = ConnectionDetails.GetSatelliteManager().ClientList;
                    foreach (SatelliteClient Client in SC)
                    {
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

        public List<Objects.SystemLock> GetLocks(List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            List<Objects.SystemLock> SystemLocks = new List<Objects.SystemLock>();

            SatelliteConnectionDetails.ForEach(ConnectionDetails =>
            {
                try
                {
                    string Environment = ConnectionDetails.GetSatelliteManager().Environment;

                    List<LockObject> lockObjects = ConnectionDetails.GetSatelliteManager().GetCurrentLocks();
                    foreach(LockObject Lock in lockObjects)
                    {
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

        public void RemoveClient(Core.Objects.Client Client)
        {
            Client.SatelliteConnection.GetSatelliteManager().RemoveClient(Client.SesstionID);
        }

        public void RemoveConnection(Core.Objects.ActiveConnection ActiveConnection)
        {
            ActiveConnection.SatelliteConnection.GetSatelliteManager().RemoveClient(ActiveConnection.RequestSessionId);
        }

        public void RemoveClientsOlderThan(int Days, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetClients(SatelliteConnectionDetails).Where(Client => Client.DaysOld > Days).ToList().ForEach(Client =>
            {
                RemoveClient(Client);
            });
        }

        public void RemoveClientsByUserID(string UserID, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetClients(SatelliteConnectionDetails).Where(Client => Client.UserId == UserID).ToList().ForEach(Client =>
            {
                RemoveClient(Client);
            });
        }

        public void RemoveConnectionsOldetThan(int Days, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetActiveConnections(SatelliteConnectionDetails).Where(Connection => Connection.DaysOld > Days).ToList().ForEach(Connection =>
            {
                RemoveConnection(Connection);
            });
        }

        public void RemoveConnectionByUserID(string UserID, List<Objects.SatelliteConnection> SatelliteConnectionDetails)
        {
            GetActiveConnections(SatelliteConnectionDetails).Where(Connection => Connection.ConnectionUser == UserID).ToList().ForEach(Connection =>
            {
                RemoveConnection(Connection);
            });
        }
    }
}
