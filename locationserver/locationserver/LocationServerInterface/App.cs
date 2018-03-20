using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using locationserverConsole;   
using Microsoft.Win32.SafeHandles; 
namespace LocationServerInterface
{
    public partial class App : Application
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole(); // external method to allocate the console
        [DllImport("kernel32")]
        static extern bool FreeConsole(); // external method to free the console
        void app_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Contains("-w")) // if it contains the argument to launch the UI then lanuch the UI
            {
                FreeConsole(); // ditch the console
                MainWindow window = new MainWindow(); // create the main window
                for (int x = 0; x < e.Args.Length; x++) {
                    switch (e.Args[x]) {
                        case "-d":
                            window.Debug.IsChecked = true; // enable debuging
                            break;
                        case "-p":
                            try
                            {
                                window.Port.Text = e.Args[x + 1]; // change the port number
                                if(int.Parse(e.Args[x+1]) <= 0) // check if the port is less than or equal to 0
                                {
                                    throw new Exception();
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Invalid Port number - using 43", "Error", MessageBoxButton.OK,  MessageBoxImage.Error);
                            }
                            x++;
                            break;
                        case "-l":
                            try
                            {
                                window.LogCheckBox.IsChecked = true;
                                window.LogPath.Text = e.Args[x + 1]; // activate the log path in the UI
                            }
                            catch {
                                MessageBox.Show("Invalid Log Path, using default log.txt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            x++;
                            break;
                        case "-f":
                            try
                            {
                                window.DatabaseCustomBox.IsChecked = true;
                                window.PathBox.Text = e.Args[x + 1];
                            }
                            catch {
                                MessageBox.Show("Invalid dictionary path, using dictionary.txt instead", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            x++;
                            break;
                        case "-t":
                            try
                            {
                                
                                if (int.Parse(e.Args[x + 1]) < 0)
                                {
                                    throw new Exception();
                                }
                                window.TimeoutText.Text = e.Args[x + 1];
                            }
                            catch
                            {
                                MessageBox.Show("The Timeout is invalid - it must be a valid positive whole number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            break;
                    }
                }
                window.Show(); // show the window
                window.Activate(); // activate the window
            }
            else {
                AllocConsole(); // add the console if it is not open
                
                
                Program.createTCPListener(); // create the tcp listener
                Program.Main(e.Args); // pass constrol to the console app
                
            }
        }
        

    }
}
