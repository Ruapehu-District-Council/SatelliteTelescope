using AutoFormGenorator.Object;
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
        private Core.SettingsLogic SettingsLogic;
        private List<Core.Objects.SatelliteConnection> SatelliteConnections { get; set; } = new List<Core.Objects.SatelliteConnection>();

        public ManageSatelliteConnections(Core.SettingsLogic SettingsLogic)
        {
            InitializeComponent();
            this.SettingsLogic = SettingsLogic;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SatelliteConnections = SettingsLogic.LoadSatelliteConnections();
            ConnectionsStackPanel.Children.Add(SettingsLogic.GetAFGControl(SatelliteConnections));
        }

        private void SaveSettinsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsLogic.SaveConnections(SatelliteConnections);

            this.DialogResult = true;
            this.Close();
        }
    }
}

