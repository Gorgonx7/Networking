using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using locationserver;
namespace LocationServerInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Log LogWindow = new Log();
        bool LogIsOpen = false;
        Thread thread;
        public MainWindow()
        {
            
            InitializeComponent();
        }
        private void Start_Click(Object sender, RoutedEventArgs e) {
            if ((string)startstopbutton.Content == "Start")
            {
                // start the server
                int PortNumber = 43;
                try
                {
                    PortNumber = int.Parse(Port.Text);
                    Port.IsEnabled = false;
                }
                catch {
                    MessageBox.Show("The port but be a valid whole number");
                    return;
                }
                Program.setPort(PortNumber);
                Program.createTCPListener();
                string[] args = new string[1];
                if ((bool)Debug.IsChecked)
                {
                    args[0] = "-d";
                }
                Debug.IsEnabled = false;
                thread = new Thread(()=>Program.Main(args));
                thread.Start();
                
                StatusImage_GreenTick.Visibility = Visibility.Visible;
                StatusImage_RedCross.Visibility = Visibility.Hidden;
                startstopbutton.Content = "Stop";
            }
            else
            {
                // stop the server
                Port.IsEnabled = true;
                Debug.IsEnabled = true;
                Program.CloseConnection();
                thread.Abort();
                StatusImage_GreenTick.Visibility = Visibility.Hidden;
                StatusImage_RedCross.Visibility = Visibility.Visible;
                startstopbutton.Content = "Start";
            }
        }
        private void Openlog_Click(object sender, RoutedEventArgs e)
        {
            if (!LogIsOpen)
            {
                LogWindow.Activate();
                LogWindow.Show();
                LogIsOpen = true;
            }
            else {
                LogWindow.Visibility = Visibility.Hidden;
                LogIsOpen = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogWindow.Close();
        }
    }
}
