using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
namespace locationserverConsole
{
    public class Program
    {
        public static ElementManager m_Manager = new ElementManager();
        private static TcpListener m_Listener;
        private static int m_Port = 43;
        public static bool m_Debug = false;
        private static Socket m_Connection;
        private static bool isSavingFile = false;
        private static string SaveFilePath;
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
                Log.SaveLog();
                return true; 
            }
            catch {
                return false;
            }
        }
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            for (int x = 0; x < args.Length; x++) {
                if(args[x] == "-d") {
                    m_Debug = true;
                    Console.WriteLine("Started in Debug Mode");
                }
                if (args[x] == "-p") {
                    try
                    {
                        m_Port = int.Parse(args[x + 1]);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                    x++;
                    Console.WriteLine("Opening on port " + m_Port);
                    createTCPListener();
                }
                if (args[x] == "-l") {
                    Log.UpdateLogLocation(args[x + 1]);
                }
                if (args[x] == "-f") {
                    try
                    {
                        m_Manager.LoadElements(args[x + 1]);
                    }
                    catch {

                    }
                    isSavingFile = true;
                    SaveFilePath = args[x + 1];
                }
            }
            
           
            try
            {
                if (m_Listener == null) {
                    createTCPListener();
                }
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
                    counter++;
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
        public static void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                Log.SaveLog();
            }
            catch
            {
                
            }
            if (isSavingFile)
            {
                try
                {
                    m_Manager.SaveElements(SaveFilePath);
                }
                catch
                {

                }
            }
        }



    }
}
