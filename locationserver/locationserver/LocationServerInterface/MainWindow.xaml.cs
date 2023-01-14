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
using System.Runtime.InteropServices;

namespace LocationServerInterface
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole(); // external method to allocate the console
        [DllImport("kernel32")]
        static extern bool FreeConsole();
        Log LogWindow = new Log(); // create the log window 
        public static bool LogIsOpen = false;
        Thread thread;
        public MainWindow()
        {

            InitializeComponent();
        }
        /// <summary>
        /// starts the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(Object sender, RoutedEventArgs e)
        {
            // if the server has not been started
            if ((string)startstopbutton.Content == "Start")
            {
                // start the server
                int PortNumber = 43; // default port
                try
                {
                    PortNumber = int.Parse(Port.Text);

                    if (PortNumber <= 0) // validate the port number
                    {
                        throw new Exception();
                    }
                    Port.IsEnabled = false; // disable the UI
                }
                catch
                {
                    MessageBox.Show("The port but be a valid positive whole number that is greater than 0", "Error Message", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return; // cancel start
                }
                Program.SetPort(PortNumber); // set the port in the console app
                Program.createTCPListener(); // create the tcp listener in the console app
                string[] args = new string[1]; // phase the args to be passed to the program
                if ((bool)Debug.IsChecked)
                {
                    args[0] = "-d"; // if it is debuging
                }
                Debug.IsEnabled = false; // remove control from the user
                thread = new Thread(() => Program.Main(args)); // start the server
                thread.Start(); // start the thread
                // inform the user of the server status
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
                Program.CloseConnection(); // close the connection
                thread.Abort(); // abort the current server thread
                // inform the user of the current server state
                StatusLable.Content = "The server is not running";
                StatusImage_GreenTick.Visibility = Visibility.Hidden;
                StatusImage_RedCross.Visibility = Visibility.Visible;
                startstopbutton.Content = "Start";
            }
        }
        /// <summary>
        /// Opens the log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Openlog_Click(object sender, RoutedEventArgs e)
        {

            if (!LogIsOpen)
            {
                try
                {
                    // try to show the log if it has been closed properly
                    LogWindow.Activate();
                    LogWindow.Show();
                }
                catch
                {
                    //force the log window to update
                    LogWindow = new Log();
                    LogWindow.Activate();
                    LogWindow.Show();
                }
                LogIsOpen = true;
            }
            else
            {
                //hide but don't close the window
                LogWindow.Visibility = Visibility.Hidden;
                LogIsOpen = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LogWindow.Close(); //close the log window
            try
            {
                Program.CloseConnection(); // abort any current connection
            }
            catch
            {

            }
            if (thread != null)
            {
                try
                {
                    thread.Abort(); // abort the current server thread
                }
                catch
                {

                }
            }

            if (PathBox.IsEnabled) // save if the user specifies it to be saved
            {

                try
                {
                    Program.isSavingFile = true;
                    Program.SaveFilePath = PathBox.Text; // ensure the save paths are up to date
                                                         // locationserverConsole.Program.m_Manager.SaveElements(PathBox.Text);
                }
                catch
                {
                    MessageBox.Show("Failed to find custom directory specified \n Have you defined it correctly?"); // if there is an issue respond
                }

            }

        }
        /// <summary>
        /// dictionary save checkbox for both check and uncheck
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!PathBox.IsEnabled)
            {
                // enable the changing of the directory
                PathBox.IsEnabled = true;
                DictionaryButton.IsEnabled = true;
                Program.isSavingFile = true;
                Program.SaveFilePath = PathBox.Text;
            }
            else
            {
                //diable the changing of the directory
                PathBox.IsEnabled = false;
                DictionaryButton.IsEnabled = false;
                Program.isSavingFile = false;
                Program.SaveFilePath = PathBox.Text;
            }
        }
        /// <summary>
        /// used to change the log save location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            // if the log path is enabled disable it and reset the log path
            if (LogPath.IsEnabled)
            {
                LogPath.IsEnabled = false;
                locationserverConsole.Log.UpdateLogLocation("log.txt");
                LogButton.IsEnabled = false;
            }
            else
            {
                //else enable the log path box and update the log location to the location specified within
                LogPath.IsEnabled = true;
                locationserverConsole.Log.UpdateLogLocation(LogPath.Text);
                LogButton.IsEnabled = true;
            }
        }
        /// <summary>
        /// Opens the windows dialog for selecting a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            // use windows forms file dialog to set the path
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Text|*.txt|All|*.*|Log|*.log"; // filter the files to valid extensions
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    //change the log path text box to the result path
                    LogPath.Text = System.IO.Path.GetFullPath(dialog.FileName);
                    MessageBox.Show("Location of log changed to: " + LogPath.Text, "Load", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }


            }
        }
        /// <summary>
        /// functionally identical to the method above just for specifiying the dictionary directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Text|*.txt|All|*.*|Dat|*.dat"; // different file filters
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {

                    PathBox.Text = System.IO.Path.GetFullPath(dialog.FileName);
                    MessageBox.Show("Location of log changed to: " + PathBox.Text, "Load", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }


            }

        }
        /// <summary>
        /// if the path box is changed call this
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Program.isSavingFile = true;
            Program.SaveFilePath = PathBox.Text; // update the current save path
        }
        /// <summary>
        /// the same as the above just for the log path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            locationserverConsole.Log.UpdateLogLocation(LogPath.Text); // update the log location
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Program.m_Timeout = int.Parse(TimeoutText.Text);
                if (Program.m_Timeout < 0) // verify the timeout
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Invalid Port Number - please use a postitive whole number in ms, use 0 to disable timeouts");
                TimeoutText.Text = "1000";
            }
        }

        private void Debug_Checked(object sender, RoutedEventArgs e)
        {
           
                AllocConsole();
            
        }

        private void Debug_Unchecked(object sender, RoutedEventArgs e)
        {
            FreeConsole();
        }
    }
}
