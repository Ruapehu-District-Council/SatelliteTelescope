using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SatelliteHelperTool.Displays
{
    /// <summary>
    /// Interaction logic for ManageSatelliteConnections.xaml
    /// </summary>
    public partial class ManageSatelliteConnections : Window
    {
        //Set the Setting logic
        private Core.SettingsLogic SettingsLogic;

        private List<Core.Objects.SatelliteConnection> SatelliteConnections { get; set; } = new List<Core.Objects.SatelliteConnection>();

        public ManageSatelliteConnections(Core.SettingsLogic SettingsLogic)
        {
            InitializeComponent();
            //Use and instace of the settings object the main form has
            this.SettingsLogic = SettingsLogic;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load the connections from the settings file and show the AFG control
            SatelliteConnections = SettingsLogic.LoadSatelliteConnections();
            ConnectionsStackPanel.Children.Add(SettingsLogic.GetAFGControl(SatelliteConnections));
        }

        private void SaveSettinsButton_Click(object sender, RoutedEventArgs e)
        {
            //Save the connections and close the window.
            SettingsLogic.SaveConnections(SatelliteConnections);

            this.DialogResult = true;
            this.Close();
        }
    }
}

