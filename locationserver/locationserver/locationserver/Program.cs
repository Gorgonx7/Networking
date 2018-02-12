using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
namespace locationserver
{
    class Program
    {
        static ElementManager m_Manager = new ElementManager();
        static void Main(string[] args)
        {
            TcpListener m_Listener;
            Socket m_Connection;
            NetworkStream m_SocketStream;
            

            try
            {
                m_Listener = new TcpListener(IPAddress.Any, 43);
                m_Listener.Start();
                while (true)
                {
                    
                    m_Connection = m_Listener.AcceptSocket();
                    m_SocketStream = new NetworkStream(m_Connection);
                    StreamReader sr = new StreamReader(m_SocketStream);
                    StreamWriter sw = new StreamWriter(m_SocketStream);
                    
                  
                    sw.WriteLine(doRequest(sr.ReadLine()));
                    sw.Flush();
                    m_SocketStream.Close();
                    m_Connection.Close();
                    //m_Manager.SaveElements();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);

            }
        }

        public static string doRequest(string pRequest) {
            string Output = "";
            /*
             * Name | Location
             */
            string[] SplitRequest = pRequest.Split(' ');
            if (SplitRequest.Length == 2) {
                if (m_Manager.UpdateLocation(SplitRequest[0], SplitRequest[1])) {
                    Output = "OK\r\n";
                }
            }
            if (SplitRequest.Length == 1) {
                Output = m_Manager.GetLocation(SplitRequest[0]);
            }
            return Output;
        } 

    }
}
