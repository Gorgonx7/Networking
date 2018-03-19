using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace locationserverConsole
{
    public class Program
    {
        public static ElementManager m_Manager = new ElementManager();
        private static TcpListener m_Listener;
        private static int m_Port = 43;
        public static bool m_Debug = false;
        private static Socket m_Connection;
        public static bool isSavingFile = false;
        public static string SaveFilePath;
       
#region endmanager
        [DllImport("kernel32")]
        static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes

        {

            CTRL_C_EVENT = 0,

            CTRL_BREAK_EVENT,

            CTRL_CLOSE_EVENT,

            CTRL_LOGOFF_EVENT = 5,

            CTRL_SHUTDOWN_EVENT

        }
#endregion 
        public static void createTCPListener() {
            m_Listener = new TcpListener(IPAddress.Any, m_Port);
        }
        public static void SetPort(int port) {
            m_Port = port;
        }
        public static bool CloseConnection() {
            try
            {
                m_Listener.Stop();
                m_Connection.Close();
                
                //Log.SaveLog();
                return true; 
            }
            catch {
                return false;
            }
        }
        public static void Main(string[] args)
        {
            bool error = false;
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

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
                    
                    try
                    {
                        Log.UpdateLogLocation(args[x + 1]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        error = true;
                    }
                    
                }
                if (args[x] == "-f") {
                    try
                    {
                        if (m_Debug) {
                            Console.WriteLine("Saving elements to location: " + args[x + 1]);
                            Thread.Sleep(3000);
                        }
                        if(args[x + 1] == "")
                        {
                            continue;
                        }
                        m_Manager.LoadElements(args[x + 1]);
                        
                    }
                    catch (Exception e)
                    {
                        if (m_Debug)
                        {
                            Console.WriteLine(e);
                            //error = true;
                        }
                    }
                    try
                    {
                    
                    
                        SaveFilePath = args[x + 1];
                        isSavingFile = true;
                    }
                    catch {
                        error = true;
                    }
                }
            }

            if (!error)
            {
                try
                {
                    if (m_Listener == null)
                    {
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
                finally
                {
                    Log.SaveLog();
                }
            }

        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)

        {

            // Put your own handler here

            switch (ctrlType)

            {

                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:                    
                case CtrlTypes.CTRL_CLOSE_EVENT:                  
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    OnProcessExit();
                    break;



            }

            return true;

        }

        public static void OnProcessExit()
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
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }



    }
}
