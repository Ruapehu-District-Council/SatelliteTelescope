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
        //Setup the logic classes
        private Core.SettingsLogic SettingsLogic;
        private Core.DataLogic DataLogic;

        //List of Satellite Connections
        private List<Core.Objects.SatelliteConnection> SatelliteConnections;

        //Update Timer for Data
        private System.Windows.Threading.DispatcherTimer UpdateTimer;

        public MainWindow()
        {
            SettingsLogic = new Core.SettingsLogic();
            DataLogic = new Core.DataLogic();

            //Get Arguments
            string[] args = Environment.GetCommandLineArgs();

            //If there is more than the default number of arguments, Handle them
            if (args.Length > 1)
            {
                //Set running as CLI to stop popups.
                DataLogic.RunningAsCLI = true;

                //Connect to the Satellites
                ConnectToSatellites();

                for (int i = 1; args.Length > i; i++)
                {
                    string CommandValue = "";

                    //Switch over the args to find any that it knows about.
                    switch (args[i].ToLower())
                    {
                        case "-removeclientsolderthan":
                            //Advance the count to find the value for the arg
                            i++;
                            CommandValue = args[i];
                            //See if it is a number
                            if (Regex.IsMatch(CommandValue, @"^\d$"))
                            {
                                int Days = int.Parse(CommandValue);

                                if (Days > 0)
                                {
                                    //Find and remove the clients
                                    DataLogic.RemoveClientsOlderThan(Days, SatelliteConnections);
                                }
                            }
                            break;
                        case "-removeconnectionsolderthan":
                            //Advance the count to find the value for the arg
                            i++;
                            CommandValue = args[i];
                            //See if it is a number
                            if (Regex.IsMatch(CommandValue, @"^\d$"))
                            {
                                int Days = int.Parse(CommandValue);

                                if (Days > 0)
                                {
                                    //Find and remove the connection
                                    DataLogic.RemoveConnectionsOldetThan(Days, SatelliteConnections);
                                }
                            }
                            break;
                    }
                }
                //Disconnect from the Satellites
                DisconnectFromSatellites();

                //Close the app
                this.Close();
                return;
            }
            else
            {
                InitializeComponent();
            }
        }

        //Shows the ManageSatelliteConnections display so you can add or remove connections 
        private void ManageConnectionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Displays.ManageSatelliteConnections ManageSatelliteConnections = new Displays.ManageSatelliteConnections(SettingsLogic);

            ManageSatelliteConnections.ShowDialog();

            //Drop the current connections and connect to new ones.
            DisconnectFromSatellites();
            ConnectToSatellites();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Connect and setup the timer as well as starting it.
            ConnectToSatellites();
            SetupTimer();
        }

        //Load the connections from the settings file, and connect
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

        //Disconnect
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

        //Setup the Timer that probes for changes
        private void SetupTimer()
        {
            UpdateTimer = new System.Windows.Threading.DispatcherTimer();
            UpdateTimer.Tick += UpdateTimer_Tick; ;
            UpdateTimer.Interval = new TimeSpan(0, 0, 1);
            UpdateTimer.Start();

            //Set the Menu item in the File menu to say pause because it's running.
            TimerControlMenuItem.Header = "Pause";
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            //Iterate Through the connections
            SatelliteConnections.ForEach(SatelliteConnection =>
            {
                try
                {
                    //see if the connection is connected
                    if (SatelliteConnection.Connected)
                    {
                        //Get the data from the connection and poopulate the listviews
                        ActiveConnectionsListView.ItemsSource = DataLogic.GetActiveConnections(SatelliteConnections);
                        ClientsListView.ItemsSource = DataLogic.GetClients(SatelliteConnections).OrderByDescending(a => a.DaysOld).ToList(); //Sort this by how old the client is.
                        LocksistView.ItemsSource = DataLogic.GetLocks(SatelliteConnections);
                    }
                }
                catch
                {

                }
            });
        }

        //Handle when a user clicks on Active Connections -> Remove Connections -> Older than x Days
        private void ActiveConnectionRemoveOlderThan_Click(object sender, RoutedEventArgs e)
        {
            //We are using the tag to store how many days to remove by
            MenuItem menuItem = (MenuItem)sender;
            int OlderThan = int.Parse(menuItem.Tag.ToString());

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove Connections older than " + OlderThan + " days?", "Remove Connections?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //Remove the connections
                DataLogic.RemoveConnectionsOldetThan(OlderThan, SatelliteConnections);
            }
        }

        //Handle when a user clicks on Active Connections -> Remove Client -> Older than x Days
        private void ClientsRemoveOlderThan_Click(object sender, RoutedEventArgs e)
        {
            //We are using the tag to store how many days to remove by
            MenuItem menuItem = (MenuItem)sender;
            int OlderThan = int.Parse(menuItem.Tag.ToString());

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove Clients older than "+ OlderThan + " days?", "Remove Clients?", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //Remove the Clients
                DataLogic.RemoveClientsOlderThan(OlderThan, SatelliteConnections);
            }
        }

        //Handle click of remove in the clients list view
        private void RemoveClientButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Makesure that the left button was proessed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Core.Objects.Client Client = ((TextBlock)sender).DataContext as Core.Objects.Client;

                //Makesure that we have a client object, and that the recored wasn't deleted.
                if (Client == null)
                {
                    return;
                }

                //Question the user if this is what they wanted to do.
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove this connection " + Client.UserId + "?", "Remove Connetion?", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    //Remove the client
                    DataLogic.RemoveClient(Client, SatelliteConnections);
                }
            }
        }

        //Handle click of remove in the Connections list view
        private void RemoveConnectionButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Makesure that the left button was proessed
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Core.Objects.ActiveConnection ActiveConnection = ((TextBlock)sender).DataContext as Core.Objects.ActiveConnection;

                //Makesure that we have a ActiveConnection object, and that the recored wasn't deleted.
                if (ActiveConnection == null)
                {
                    return;
                }

                //Question the user if this is what they wanted to do.
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to remove this connection " + ActiveConnection.RequestUserId + "?", "Remove Connetion?", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    //Remove the Connection
                    DataLogic.RemoveConnection(ActiveConnection);
                }
            }
        }

        //Handle the timer control in the File menu so you can "Pause" and "Resume" it
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

        //Handle the remve Client by UserID in File -> Client Connections -> RemoveClientByUserID
        private void RemoveClientByUserIDMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Display the window for the user to enter a user ID.
            Popups.RemoveUsingClientID RemoveUsingClientID = new Popups.RemoveUsingClientID();
            RemoveUsingClientID.ShowDialog();
            if (RemoveUsingClientID.DialogResult == true)
            {
                //Remove the clients by user ID
                DataLogic.RemoveClientsByUserID(RemoveUsingClientID.UserIDTextBox.Text, SatelliteConnections);
            }
        }

        //Handle the remve Client by custom day in File -> Client Connections -> RemoveClientByCustomDay
        private void RemoveClientByCustomDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Display Window for the user to enter a day number
            Popups.RemoveByCustom RemoveByCustom = new Popups.RemoveByCustom();
            RemoveByCustom.ShowDialog();
            if (RemoveByCustom.DialogResult == true)
            {
                //Remove the Clients after day provided
                DataLogic.RemoveClientsOlderThan(int.Parse(RemoveByCustom.DaysTextBox.Text), SatelliteConnections);
            }
        }

        //Handle the remve connection by custom day in File -> Active Connections -> RemoveClientByCustomDay
        private void RemoveConnectionByCustomDayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Display Window for the user to enter a day number
            Popups.RemoveByCustom RemoveByCustom = new Popups.RemoveByCustom();
            RemoveByCustom.ShowDialog();
            if (RemoveByCustom.DialogResult == true)
            {
                //Remove the connections after day provided
                DataLogic.RemoveConnectionsOldetThan(int.Parse(RemoveByCustom.DaysTextBox.Text), SatelliteConnections);
            }
        }

        //Handle the remve Connections by UserID in File -> Active Connections -> RemoveConnectionByUserIDM
        private void RemoveConnectionByUserIDMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Display the window for the user to enter a user ID.
            Popups.RemoveUsingClientID RemoveUsingClientID = new Popups.RemoveUsingClientID();
            RemoveUsingClientID.ShowDialog();
            if (RemoveUsingClientID.DialogResult == true)
            {
                //Remove the connections by user ID
                DataLogic.RemoveConnectionByUserID(RemoveUsingClientID.UserIDTextBox.Text, SatelliteConnections);
            }
        }

        //When the app is closing, Disconect
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectFromSatellites();
        }
    }
}
