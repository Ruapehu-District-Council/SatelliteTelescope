using System;
using System.Collections.Generic;
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

namespace SatelliteHelperTool.Popups
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RemoveUsingClientID : Window
    {
        public RemoveUsingClientID()
        {
            InitializeComponent();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //This is so you can click the headder and drag it round
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
