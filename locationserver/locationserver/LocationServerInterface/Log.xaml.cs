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
using locationserverConsole;
using System.Threading;

namespace LocationServerInterface
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : Window
    {
        
        public Log()
        {
            InitializeComponent();
            LogBox.ItemsSource = locationserverConsole.Log.GetLog(); // reference the log
            
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // notify the main window that the log is closed
            MainWindow.LogIsOpen = false;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //refresh the items in the listbox
            LogBox.Items.Refresh();
        }
    }
}
