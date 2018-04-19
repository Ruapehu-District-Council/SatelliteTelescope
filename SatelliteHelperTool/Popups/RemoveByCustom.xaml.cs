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
using System.Windows.Shapes;

namespace SatelliteHelperTool.Popups
{
    /// <summary>
    /// Interaction logic for RemoveByCustom.xaml
    /// </summary>
    public partial class RemoveByCustom : Window
    {
        public RemoveByCustom()
        {
            InitializeComponent();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Viladate())
            {
                DialogResult = false;
                this.Close();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ValueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        public bool Viladate()
        {
            bool Valid = true;
            if (DaysTextBox.Text.Length == 0)
            {
                Valid = false;
            }

            int dummy = 0;
            if (!int.TryParse(DaysTextBox.Text, out dummy))
            {
                Valid = false;
            }

            if (Valid)
            {
                DaysTextBox.BorderBrush = Brushes.Black;
            }
            else
            {
                DaysTextBox.BorderBrush = Brushes.Red;
            }

            return Valid;
        }
    }
}
