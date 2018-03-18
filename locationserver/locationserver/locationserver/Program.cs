using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
namespace locationserver
{
    public class Program
    {
        public static ElementManager m_Manager = new ElementManager();
        private static TcpListener m_Listener;
        private static int m_Port = 43;
        public static bool m_Debug = false;
        private static Socket m_Connection;
        public static void createTCPListener() {
            m_Listener = new TcpListener(IPAddress.Any, m_Port);
        }
        public static void SetPort(int port) {
            m_Port = port;
        }
        public static bool CloseConnection() {
            try
            {
               
                m_Connection.Close();
                m_Listener.Stop();
                return true;
            }
            catch {
                return false;
            }
        }
        public static void Main(string[] args)
        {
            for (int x = 0; x < args.Length; x++) {
                if(args[x] == "-d") {
                    m_Debug = true;
                }
                if (args[x] == "-p") {
                    m_Port = int.Parse(args[x + 1]);
                    x++;
                    createTCPListener();
                }
            }
            
           
            try
            {
                
                m_Listener.Start();
                Console.WriteLine(">> " + "Server Started");
                int counter = 0;
                while (true)
                {
                    m_Connection = m_Listener.AcceptSocket();
                    Console.WriteLine(" >> " + "Client No:" + counter + " started!");
                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.startClient(m_Connection, counter);
                    
                    //m_Connection.Close();

                }





                //m_Manager.SaveElements();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            finally {
                
            }


        }




    }
}
