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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LocationServerInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Log LogWindow = new Log();
        bool LogIsOpen = false;
        public MainWindow()
        {
            
            InitializeComponent();
        }
        private void Start_Click(Object sender, RoutedEventArgs e) {
            if ((string)startstopbutton.Content == "Start")
            {
                // start the server
                StatusImage_GreenTick.Visibility = Visibility.Visible;
                StatusImage_RedCross.Visibility = Visibility.Hidden;
                startstopbutton.Content = "Stop";
            }
            else
            {
                // stop the server
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
