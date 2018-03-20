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

using locationserverConsole;
using System.IO;

namespace LocationServerInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Log LogWindow = new Log();
        public static bool LogIsOpen = false;
        Thread thread;
        public MainWindow()
        {

            InitializeComponent();
        }
        private void Start_Click(Object sender, RoutedEventArgs e)
        {
            if ((string)startstopbutton.Content == "Start")
            {
                // start the server
                int PortNumber = 43;
                try
                {
                    PortNumber = int.Parse(Port.Text);
                    Port.IsEnabled = false;
                }
                catch
                {
                    MessageBox.Show("The port but be a valid whole number");
                    return;
                }
                Program.SetPort(PortNumber);
                Program.createTCPListener();
                string[] args = new string[1];
                if ((bool)Debug.IsChecked)
                {
                    args[0] = "-d";
                }
                Debug.IsEnabled = false;
                thread = new Thread(() => Program.Main(args));
                thread.Start();
                StatusLable.Content = "The server is running";
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
                StatusLable.Content = "The server is not running";
                StatusImage_GreenTick.Visibility = Visibility.Hidden;
                StatusImage_RedCross.Visibility = Visibility.Visible;
                startstopbutton.Content = "Start";
            }
        }
        private void Openlog_Click(object sender, RoutedEventArgs e)
        {
            if (!LogIsOpen)
            {
                try
                {
                    LogWindow.Activate();
                    LogWindow.Show();
                }
                catch
                {
                    LogWindow = new Log();
                    LogWindow.Activate();
                    LogWindow.Show();
                }
                LogIsOpen = true;
            }
            else
            {
                LogWindow.Visibility = Visibility.Hidden;
                LogIsOpen = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogWindow.Close();
            try
            {
                Program.CloseConnection();
            }
            catch
            {

            }
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                }
                catch
                {

                }
            }

            if (PathBox.IsEnabled)
            {

                try
                {
                    Program.isSavingFile = true;
                    Program.SaveFilePath = PathBox.Text;
                   // locationserverConsole.Program.m_Manager.SaveElements(PathBox.Text);
                }
                catch
                {
                    MessageBox.Show("Failed to find custom directory specified \n Have you defined it correctly?");
                }

            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!PathBox.IsEnabled)
            {
                PathBox.IsEnabled = true;
                DictionaryButton.IsEnabled = true;
                Program.isSavingFile = true;
                Program.SaveFilePath = PathBox.Text;
            }
            else
            {
                PathBox.IsEnabled = false;
                DictionaryButton.IsEnabled = false;
                Program.isSavingFile = false;
                Program.SaveFilePath = PathBox.Text;
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (LogPath.IsEnabled)
            {
                LogPath.IsEnabled = false;
                locationserverConsole.Log.UpdateLogLocation("log.txt");
                LogButton.IsEnabled = false;
            }
            else
            {
                LogPath.IsEnabled = true;
                locationserverConsole.Log.UpdateLogLocation(LogPath.Text);
                LogButton.IsEnabled = true;
            }
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Text|*.txt|All|*.*|Log|*.log";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    LogPath.Text = System.IO.Path.GetFullPath(dialog.FileName);
                    MessageBox.Show("Location of log changed to: " + LogPath.Text);
                }


            }
        }

        private void DictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Text|*.txt|All|*.*|Dat|*.dat";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    PathBox.Text = System.IO.Path.GetFullPath(dialog.FileName);
                    MessageBox.Show("Location of log changed to: " + PathBox.Text);
                }


            }

        }

        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Program.isSavingFile = true;
            Program.SaveFilePath = PathBox.Text;
        }

        private void LogPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            locationserverConsole.Log.UpdateLogLocation(LogPath.Text);
        }
    }
}
