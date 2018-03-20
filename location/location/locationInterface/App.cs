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
    /// <summary>
    /// Entery point for the application when launched in wpf mode, this was changed from the main window as this needs to be seperate from
    /// the constructor for mainwindow 
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32")]
        static extern bool AllocConsole(); // External import of the method that allocates the the console to the application
        [DllImport("kernel32")]
        static extern bool FreeConsole(); // External import of the method that frees that console from the application
        void app_Startup(object sender, StartupEventArgs e)
        {
            // If no command line arguments were provided, don't process them and open the wpf windows
            MainWindow window = new MainWindow(); // constructor for the main window, this needs to be created no matter what to ensure the application closes correctly
            if (e.Args.Length == 0) // check if the number of args is equal to zero 
            {
                FreeConsole(); // frees the console from the application in wpf mode
                window.Activate(); //activates the main window
                window.Show(); //shows the main window
            }
            else
            {
                //conosole mode
                ConsoleMode(e);
                System.Threading.Thread.Sleep(60); // wait for the application to start before the window is closed
                window.Close();

            }


        }


        private void ConsoleMode(StartupEventArgs e) {
            AllocConsole(); // allocate the console
            Program.Main(e.Args); //pass the control to the console version of the application
            
            
        }
    }
}
