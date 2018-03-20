using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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

namespace locationInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool m_ConsoleisOpen = false; // bool that is used to specify debugging
        public MainWindow()
        {
            InitializeComponent();
        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole(); // external method to open the console

        [DllImport("Kernel32")]
        public static extern void FreeConsole(); // external method to clsoe the console

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Portnumb; // the port for the client to send the message too
            int Timeout; // the timeout for the client side of the application
            string hostname; // the host name of the server that the message will be sent too
            if (Username.Text.Contains(" ")) { // check for spaces in the name
                MessageBox.Show("Invalid name: your name cannot contain spaces","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                Portnumb = int.Parse(Port.Text); // try to convert the port to an int, but
                if(Portnumb <= 0)
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("The port number has to be a valid positive whole number greater than 0", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // jump out of the method should use return
            }
            try
            {
                Timeout = int.Parse(TimeoutTextBox.Text);
            }
            catch
            {
                MessageBox.Show("The timeout period has to be a valid whole number","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            location.MessageProtocol messageProtocol; // define the message protocol relative to the dropdown box
            switch (Protocol.Text.Trim()) {
                
                    
                case "HTTP/0.9":
                    messageProtocol = location.MessageProtocol.HTTP9;
                    break;
                case "HTTP/1.0":
                    messageProtocol = location.MessageProtocol.HTTP1;
                    break;
                case "HTTP/1.1":
                    messageProtocol = location.MessageProtocol.HTTP11;
                    break;
                default:
                    messageProtocol = location.MessageProtocol.WhoIs;
                        break;
            }
            if ((bool)UseLocalhost.IsChecked)
            {
                hostname = "localhost"; // set the hostname to localhost if the checkbox is checked
            }
            else
            {
                hostname = Hostname.Text; // if it is not set it to the host name specified
            }
            if (!LocationBox.IsEnabled) {
                //call the lookup method
               MessageBox.Show(location.Program.WPFInitialise(Username.Text, hostname, Portnumb, Timeout, messageProtocol, (bool)isDebug.IsChecked)); 
            }
            else
            {
                //call the update method
               MessageBox.Show(location.Program.WPFInitialise(Username.Text, LocationBox.Text, hostname, Portnumb, Timeout, messageProtocol, (bool)isDebug.IsChecked));

            }

            
        }

        private void UseLocalhost_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)UseLocalhost.IsChecked) // this is due to uncheck using the same method
            {
                Hostname.IsEnabled = false;
            }
            else
            {
                Hostname.IsEnabled = true;
            }
        }

        private void IsUpdate_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)IsUpdate.IsChecked) // this is due to uncheck using the same method
            {
                LocationBox.IsEnabled = true;
            }
            else
            {
                LocationBox.IsEnabled = false;
            }
        }

        private void isDebug_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_ConsoleisOpen) // this enables debuging
            {
                AllocConsole(); // this does not bind the console so debug mode will not work unless you run without native debugging
                m_ConsoleisOpen = true;
                Console.WriteLine("Started debug mode");
            }
            else {
                FreeConsole();
                m_ConsoleisOpen = false;
            }
        }
    }
}
