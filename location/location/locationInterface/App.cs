using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using location;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace locationInterface
{
    public partial class App : Application
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole();
        [DllImport("kernel32")]
        static extern bool FreeConsole();
        void app_Startup(object sender, StartupEventArgs e)
        {
            // If no command line arguments were provided, don't process them
            MainWindow window = new MainWindow();
            if (e.Args.Length == 0)
            {
                FreeConsole();
                window.Activate();
                window.Show();
            }
            else
            {

                ConsoleMode(e);
                System.Threading.Thread.Sleep(60);
                window.Close();

            }


        }


        private void ConsoleMode(StartupEventArgs e) {
            AllocConsole();
            Program.Main(e.Args);
            
            
        }
    }
}
