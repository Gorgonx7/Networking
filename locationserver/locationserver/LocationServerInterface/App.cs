using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using locationserver;
namespace LocationServerInterface
{
    public partial class App : Application
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        void app_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Contains("-w"))
            {
                MainWindow window = new MainWindow();
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
