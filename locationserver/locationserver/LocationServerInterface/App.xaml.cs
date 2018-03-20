﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LocationServerInterface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            locationserverConsole.Program.OnProcessExit(); // save the log and dictionary
            //locationserverConsole.Program.m_Manager.SaveElements();
        }
    }
}
