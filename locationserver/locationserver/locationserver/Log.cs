using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace locationserverConsole
{
    public static class Log
    {
        private static string m_Filelocation = "log.txt";
        public static void UpdateLogLocation(string path)
        {
            m_Filelocation = path;
        }

        private static void AddLog(string log) {
            //handle the formatting with time and everything here
        }

    }
}
