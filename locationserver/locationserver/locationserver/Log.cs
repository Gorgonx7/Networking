using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace locationserverConsole
{
    public static class Log
    {
        private static string m_Filelocation = "log.txt"; // the log save location
        private static List<string> m_Log = new List<string>(); // the log itself

        public static List<string> GetLog() {
            return m_Log; // passed to wpf
        }

        public static void UpdateLogLocation(string path)
        {
            m_Filelocation = path; // used to change the log location
        }
        /// <summary>
        /// Used to add the log to the log list
        /// </summary>
        /// <param name="log">The log message itself</param>
        /// <param name="connection">Used to get the IP address of the client</param>
        public static void AddLog(string log, Socket connection ) {
            //handle the formatting with time and everything here
            string output = "";
            IPEndPoint remoteIpEndPoint = connection.RemoteEndPoint as IPEndPoint; // used to get the IP of the client
            if(remoteIpEndPoint != null)
            {
                output += remoteIpEndPoint.Address;
            }
            output += " - - [" + DateTime.Now + "] " + log; // phase the message
            m_Log.Add(output); // add it to the list
            
        }
        /// <summary>
        /// used to save the log when the program closes
        /// </summary>
        public static void SaveLog()
        {

            StreamWriter sw = new StreamWriter(m_Filelocation);
            foreach (string i in m_Log)
            {
                sw.WriteLine(i);
            }
            sw.Flush();
            sw.Close();
        }
            
            
            

    }
}
