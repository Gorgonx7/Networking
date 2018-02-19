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
    class Program
    {
        public static ElementManager m_Manager = new ElementManager();

        static void Main(string[] args)
        {
            TcpListener m_Listener;
            Socket m_Connection;

            try
            {
                m_Listener = new TcpListener(IPAddress.Any, 43);
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
