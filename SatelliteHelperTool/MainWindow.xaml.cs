using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatelliteHelperTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core.SettingsLogic SettingsLogic;
        private Core.DataLogic DataLogic;

        private List<Core.Objects.SatelliteConnection> SatelliteConnections;

        private System.Windows.Threading.DispatcherTimer UpdateTimer;

        public MainWindow()
        {
            SettingsLogic = new Core.SettingsLogic();
            DataLogic = new Core.DataLogic();

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                ConnectToSatellites();
                for (int i = 1; args.Length > i; i++)
                {
                    string CommandValue = "";
                    switch (args[i].ToLower())
                    {
                        case "-removeclientsolderthan":
                            i++;
                            CommandValue = args[i];
                            if (Regex.IsMatch(CommandValue, @"^\d$"))
                            {
                                DataLogic.RemoveClientsOlderThan(int.Parse(CommandValue), SatelliteConnections);
                            }
                            break;
                        case "-removeconnectionsolderthan":
                            i++;
                            CommandValue = args[i];
                            if (Regex.IsMatch(CommandValue, @"^\d$"))
                            {
                                DataLogic.RemoveConnectionsOldetThan(int.Parse(CommandValue), SatelliteConnections);
                            }
                            break;
                    }
                }
                DisconnectFromSatellites();


                this.Close();
                return;
            }
            else
            {
                InitializeComponent();
            }
        }

        private void ManageConnectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Displays.ManageSatelliteConnections ManageSatelliteConnections = new Displays.ManageSatelliteConnections(SettingsLogic);

            ManageSatelliteConnections.ShowDialog();
            DisconnectFromSatellites();
            ConnectToSatellites();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectToSatellites();
            SetupTimer();
        }

        private void ConnectToSatellites()
        {
            try
            {
                SatelliteConnections = SettingsLogic.LoadSatelliteConnections();
                DataLogic.Connect(SatelliteConnections);
            }
            catch
            {

            }
        }

        private void DisconnectFromSatellites()
        {
            try
            {
                DataLogic.Disconnect(SatelliteConnections);
            }
            catch
            {

            }
        }

        private void SetupTimer()
        {
            UpdateTimer = new System.Windows.Threading.DispatcherTimer();
            UpdateTimer.Tick += UpdateTimer_Tick; ;
            UpdateTimer.Interval = new TimeSpan(0, 0, 1);
            UpdateTimer.Start();
            TimerControlMenuItem.Header = "Pause";
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SatelliteConnections.ForEach(SatelliteConnection =>
            {
                try
                {
                    if (SatelliteConnection.Connected)
                    {
                        ActiveConnectionsListView.ItemsSource = DataLogic.GetActiveConnections(SatelliteConnections);
                        ClientsListView.ItemsSource = DataLogic.GetClients(SatelliteConnections).OrderByDescending(a => a.DaysOld).ToList();
                        LocksistView.ItemsSource = DataLogic.GetLocks(SatelliteConnections);
                    }
                }
                catch
                {

                }
            });
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Core.Objects.Client Client = ((TextBlock)sender).DataContext as Core.Objects.Client;

                if (Client == null)
                {
                    return;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove this connection "+Client.UserId+"?","Remove Connetion?",MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataLogic.RemoveClient(Client);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectFromSatellites();
        }

        private void ActiveConnectionRemoveOlderThan_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            int OlderThan = int.Parse(menuItem.Tag.ToString());

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove Connections older than " + OlderThan + " days?", "Remove Connections?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DataLogic.RemoveConnectionsOldetThan(OlderThan, SatelliteConnections);
            }
        }

        private void ClientsRemoveOlderThan_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            int OlderThan = int.Parse(menuItem.Tag.ToString());

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove Clients older than "+ OlderThan + " days?", "Remove Clients?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DataLogic.RemoveClientsOlderThan(OlderThan, SatelliteConnections);
            }
        }

        private void RemoveConnectionButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Core.Objects.ActiveConnection ActiveConnection = ((TextBlock)sender).DataContext as Core.Objects.ActiveConnection;

                if (ActiveConnection == null)
                {
                    return;
                }

                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove this connection " + ActiveConnection.RequestUserId + "?", "Remove Connetion?", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataLogic.RemoveConnection(ActiveConnection);
                }
            }
        }

        private void TimerControlMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TimerControlMenuItem.Header.ToString() == "Pause")
            {
                UpdateTimer.Stop();
                TimerControlMenuItem.Header = "Resume";
            }
            else
            {
                UpdateTimer.Start();
                TimerControlMenuItem.Header = "Pause";
            }
        }

        private void RemoveClientByUserIDMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Popups.RemoveUsingClientID RemoveUsingClientID = new Popups.RemoveUsingClientID();
            RemoveUsingClientID.ShowDialog();
            if (RemoveUsingClientID.DialogResult == true)
            {
                DataLogic.RemoveClientsByUserID(RemoveUsingClientID.UserIDTextBox.Text, SatelliteConnections);
            }
        }

        private void RemoveClientByCustomDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Popups.RemoveByCustom RemoveByCustom = new Popups.RemoveByCustom();
            RemoveByCustom.ShowDialog();
            if (RemoveByCustom.DialogResult == true)
            {
                DataLogic.RemoveClientsOlderThan(int.Parse(RemoveByCustom.DaysTextBox.Text), SatelliteConnections);
            }
        }

        private void RemoveConnectionByCustomDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Popups.RemoveByCustom RemoveByCustom = new Popups.RemoveByCustom();
            RemoveByCustom.ShowDialog();
            if (RemoveByCustom.DialogResult == true)
            {
                DataLogic.RemoveConnectionsOldetThan(int.Parse(RemoveByCustom.DaysTextBox.Text), SatelliteConnections);
            }
        }

        private void RemoveConnectionByUserIDMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Popups.RemoveUsingClientID RemoveUsingClientID = new Popups.RemoveUsingClientID();
            RemoveUsingClientID.ShowDialog();
            if (RemoveUsingClientID.DialogResult == true)
            {
                DataLogic.RemoveConnectionByUserID(RemoveUsingClientID.UserIDTextBox.Text, SatelliteConnections);
            }
        }
    }
}
