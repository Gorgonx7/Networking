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
        static extern bool AllocConsole();
        [DllImport("kernel32")]
        static extern bool FreeConsole();
        void app_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Contains("-w"))
            {
                FreeConsole();
                MainWindow window = new MainWindow();
                for (int x = 0; x < e.Args.Length; x++) {
                    switch (e.Args[x]) {
                        case "-d":
                            window.Debug.IsChecked = true;
                            break;
                        case "-p":
                            try
                            {
                                window.Port.Text = e.Args[x + 1];
                                
                            }
                            catch
                            {
                                MessageBox.Show("Invalid Port number - using 43");
                            }
                            x++;
                            break;
                        case "-l":
                            try
                            {
                                window.LogCheckBox.IsChecked = true;
                                window.LogPath.Text = e.Args[x + 1];
                            }
                            catch {
                                MessageBox.Show("Invalid Log Path, using default log.txt");
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
                                MessageBox.Show("Invalid command path, using dictionary.txt instead");
                            }
                            x++;
                            break;
                    }
                }
                window.Show();
                window.Activate();
            }
            else {
                AllocConsole();
                
                
                Program.createTCPListener();
                Program.Main(e.Args);
                
            }
        }
        

    }
}
