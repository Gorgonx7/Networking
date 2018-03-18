using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using locationserver;   
using Microsoft.Win32.SafeHandles; 
namespace LocationServerInterface
{
    public partial class App : Application
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        void app_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Contains("-w"))
            {
                MainWindow window = new MainWindow();
                for (int x = 0; x < e.Args.Length; x++) {
                    switch (e.Args[x]) {
                        case "-d":
                            window.Debug.IsChecked = true;
                            break;
                        case "-p":
                            window.Port.Text = e.Args[x + 1];
                            break;
                        case "-l":
                            break;
                        case "-f":
                            break;
                    }
                }
                window.Show();
                window.Activate();
            }
            else {
                AllocConsole();
                // stdout's handle seems to always be equal to 7
                
                Program.createTCPListener();
                Program.Main(e.Args);
            }
        }

    }
}
