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
        public static ElementManager m_Manager = new ElementManager(); // create the database
        private static TcpListener m_Listener; // the listener for the server
        private static int m_Port = 43; // the port the server is set to listen on
        public static bool m_Debug = false; // if the server is in debug mode
        private static Socket m_Connection; // the connection to the client via sockets
        public static bool isSavingFile = false; // if the database is saving
        public static string SaveFilePath; // the path to save the database too
        public static int m_Timeout = 1000; // the timeout period
#region endmanager
        [DllImport("kernel32")]
        static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add); // external method that handles when the console app closes
        public delegate bool HandlerRoutine(CtrlTypes CtrlType); // delegate to define the handler for when the server closes
        public enum CtrlTypes // an enum to hold the types of events that cause the server to close

        {

            CTRL_C_EVENT = 0,

            CTRL_BREAK_EVENT,

            CTRL_CLOSE_EVENT,

            CTRL_LOGOFF_EVENT = 5,

            CTRL_SHUTDOWN_EVENT

        }
#endregion 
        public static void createTCPListener() {
            m_Listener = new TcpListener(IPAddress.Any, m_Port); // create the TCP listener
        }
        public static void SetPort(int port) {
            m_Port = port;
        }
        public static bool CloseConnection() {
            try
            {
                m_Listener.Stop(); // stop the server
                m_Connection.Close();
                
                //Log.SaveLog();
                return true; 
            }
            catch {
                return false;
            }
        }
        /// <summary>
        /// used by the wpf and by the console version to handle all the initalisations
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            bool error = false; // if this is true there has been an error and the server should not start
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true); // sets the exit monitor method
            // process all the arugments fead to it
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
                        return;
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
                        return;
                    }
                }
                if(args[x] == "-t")
                {
                    try
                    {
                        m_Timeout = int.Parse(args[x + 1]);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + " -- Invalid timeout number");
                        return;
                    }
                }
            }

            if (!error)
            {
                //if there has not been an error
                try
                {
                    if (m_Listener == null) // if the server is in console mode
                    {
                        createTCPListener(); // create the tcp listener
                    }
                    m_Listener.Start(); // start listening
                    if (m_Debug)
                    {
                        Console.WriteLine(">> " + "Server Started");
                    }
                    int counter = 0; // start the client counter
                    while (true)
                    {
                        m_Connection = m_Listener.AcceptSocket();
                        if (m_Debug)
                        {
                            Console.WriteLine(" >> " + "Client No:" + counter + " started!");
                        }
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

        /// <summary>
        /// This handles when the application exits and allows me to gracefully exit the application
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)

        {

            

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

        /// <summary>
        /// this saves the logs and the dictionary
        /// </summary>
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
