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
        private bool m_ConsoleisOpen = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int Portnumb;
            int Timeout;
            string hostname;
            try
            {
                Portnumb = int.Parse(Port.Text);
            }
            catch
            {
                MessageBox.Show("The port number has to be a valid whole number");
                goto Escape;
            }
            try
            {
                Timeout = int.Parse(TimeoutTextBox.Text);
            }
            catch
            {
                MessageBox.Show("The timeout period has to be a valid whole number");
                goto Escape;
            }
            location.MessageProtocol messageProtocol;
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
                hostname = "localhost";
            }
            else
            {
                hostname = Hostname.Text;
            }
            if (!LocationBox.IsEnabled) {
               MessageBox.Show(location.Program.WPFInitialise(Username.Text, hostname, Portnumb, Timeout, messageProtocol, (bool)isDebug.IsChecked));
            }
            else
            {
               MessageBox.Show(location.Program.WPFInitialise(Username.Text, LocationBox.Text, Hostname.Text, Portnumb, Timeout, messageProtocol, (bool)isDebug.IsChecked));

            }

            Escape:;
        }

        private void UseLocalhost_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)UseLocalhost.IsChecked)
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
            if ((bool)IsUpdate.IsChecked)
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
            if (!m_ConsoleisOpen)
            {
                AllocConsole();
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
