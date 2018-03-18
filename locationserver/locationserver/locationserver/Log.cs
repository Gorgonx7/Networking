using System;
using System.Collections.Generic;
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
        private static string m_Filelocation = "log.txt";
        private static List<string> m_Log = new List<string>();
        public static void UpdateLogLocation(string path)
        {
            m_Filelocation = path;
        }

        public static void AddLog(string log, Socket connection ) {
            //handle the formatting with time and everything here
            string output = "";
            IPEndPoint remoteIpEndPoint = connection.RemoteEndPoint as IPEndPoint;
            if(remoteIpEndPoint != null)
            {
                output += remoteIpEndPoint.Address;
            }
            output += " - - [" + DateTime.Now + "]" + log;
            m_Log.Add(output);
        }
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
