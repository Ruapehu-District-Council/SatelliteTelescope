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
        private string ConnectionsFile = "SatelliteConnections.json";

        private JObject RootJnode;

        private AutoFormGenorator.Logic AFG;

        public SettingsLogic()
        {
            AFG = new AutoFormGenorator.Logic();
        }

        public List<Core.Objects.SatelliteConnection> LoadSatelliteConnections()
        {
            List<Core.Objects.SatelliteConnection> SatelliteConnections = new List<Objects.SatelliteConnection>();
            if (File.Exists(ConnectionsFile))
            {
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
                RootJnode = new JObject
                {
                    ["SatelliteConnections"] = new JArray()
                };
                File.WriteAllText(ConnectionsFile, RootJnode.ToString());
            }

            return SatelliteConnections;
        }

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

        public AutoFormGenorator.UserControls.FormControl GetAFGControl(List<Core.Objects.SatelliteConnection> SatelliteConnections)
        {
            ShellClass ShellClass = new ShellClass
            {
                SatelliteConnections = SatelliteConnections
            };

            return AFG.BuildFormControl(ShellClass);
        }

        public class ShellClass
        {
            [FormField(Type = Types.NestedList)]
            public List<Core.Objects.SatelliteConnection> SatelliteConnections { get; set; } = new List<Core.Objects.SatelliteConnection>();
        }

    }
}
