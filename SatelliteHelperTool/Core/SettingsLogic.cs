using AutoFormGenorator.Object;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatelliteHelperTool.Core
{
    public class SettingsLogic
    {
        //Where is the settings file located
        private string ConnectionsFile = "SatelliteConnections.json";

        //Setup the root node, so when we save to know what to save
        private JObject RootJnode;

        //Setup the AFG display
        private AutoFormGenorator.Logic AFG;

        public SettingsLogic()
        {
            AFG = new AutoFormGenorator.Logic();
        }

        //Load the SatelliteConnections from the settings file
        public List<Core.Objects.SatelliteConnection> LoadSatelliteConnections()
        {
            List<Core.Objects.SatelliteConnection> SatelliteConnections = new List<Objects.SatelliteConnection>();
            if (File.Exists(ConnectionsFile))
            {
                //If the file exists, parse it and get the connections
                RootJnode = JObject.Parse(File.ReadAllText(ConnectionsFile));

                if (RootJnode["SatelliteConnections"] != null && RootJnode["SatelliteConnections"] is JArray)
                {
                    foreach (JObject item in RootJnode["SatelliteConnections"])
                    {
                        SatelliteConnections.Add(new Core.Objects.SatelliteConnection()
                        {
                            SaterliteURL = item["SaterliteURL"]?.ToString() ?? "",
                            Port = int.Parse(item["Port"]?.ToString() ?? "0")
                        });
                    }
                }
            }
            else
            {
                //If it doesn't exist create a new root node and array and save it.
                RootJnode = new JObject
                {
                    ["SatelliteConnections"] = new JArray()
                };
                File.WriteAllText(ConnectionsFile, RootJnode.ToString());
            }

            return SatelliteConnections;
        }

        //Save the connections
        public void SaveConnections(List<Core.Objects.SatelliteConnection> SatelliteConnections)
        {
            JArray Connections = new JArray();

            SatelliteConnections.ForEach(SatelliteConnection =>
            {
                Connections.Add(new JObject()
                {
                    ["SaterliteURL"] = SatelliteConnection.SaterliteURL,
                    ["Port"] = SatelliteConnection.Port
                });
            });

            RootJnode["SatelliteConnections"] = Connections;

            File.WriteAllText(ConnectionsFile, RootJnode.ToString());
        }

        //Get the AFG display for the Manage Satellite connections
        public AutoFormGenorator.UserControls.FormControl GetAFGControl(List<Core.Objects.SatelliteConnection> SatelliteConnections)
        {
            //Use the shell class as the base for AFG to display
            //Just AFG stuff it can be ignorged...
            ShellClass ShellClass = new ShellClass
            {
                SatelliteConnections = SatelliteConnections
            };

            return AFG.BuildFormControl(ShellClass);
        }

        //Shell class for AFG
        public class ShellClass
        {
            //This is just for AFG
            [FormField(Type = Types.NestedList)]
            public List<Core.Objects.SatelliteConnection> SatelliteConnections { get; set; } = new List<Core.Objects.SatelliteConnection>();
        }

    }
}
