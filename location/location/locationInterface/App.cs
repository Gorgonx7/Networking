using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using location;
namespace locationInterface
{
    public partial class App : Application
    {
        void app_Startup(object sender, StartupEventArgs e)
        {
            // If no command line arguments were provided, don't process them
            if (e.Args.Length == 0)
            {
                MainWindow window = new MainWindow();
                window.Activate();
                window.Show();
            }
            else
            {
                Program.Main(e.Args);
            }

            
        }
    }
}
