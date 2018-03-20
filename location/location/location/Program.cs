using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net;
namespace location
{
    public class Program
    {
        /// <summary>
        /// this method is used to initalise the default console application and to be called by the wpf version to start the request
        /// </summary>
        /// <param name="args"> the peramaters of the message</param>
        public static void Main(string[] args)
        {
            start(args); // start the client
            

            // legasy code incase I need to debug
           /* try
            {

                TcpClient client = new TcpClient();
                client.Connect("whois.net.dcs.hull.ac.uk", 43);
                client.GetStream().ReadTimeout = 1000;
                StreamWriter sw = new StreamWriter(client.GetStream());
                StreamReader sr = new StreamReader(client.GetStream());
                string exception = args[0];


                for (int x = 0; x < args.Length; x++)
                {
                    if (x == args.Length - 1)
                    {
                        sw.WriteLine(args[x]);
                    }
                    else
                    {
                        sw.Write(args[x] + " ");
                    }
                }

                sw.Flush();


                string holder = sr.ReadToEnd();
                if (holder == "OK\r\n" && args.Length >= 2)
                {
                    Console.WriteLine(args[0] + " location changed to be " + args[1]);
                }
                else if (holder != "ERROR: no entries found\r\n")
                {
                    Console.WriteLine(args[0] + " is " + holder.TrimEnd());
                }
                else
                {

                    Console.WriteLine(holder);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            */

        }
        /// <summary>
        /// used to start a OO request
        /// </summary>
        /// <param name="args">the arguments of the message</param>
        /// <returns>returns the strings to be passed to wpf</returns>
        public static string start(string[] args)
        {
            try
            {
                Message message = new Message(args); // try to create the message
                return MessageWrapper.SendMessage(message); // try to send the message and pass the return string upward to wpf
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return "Error unknown in settings";
            }
        }
        /// <summary>
        /// used to initalise a wpf update
        /// </summary>
        /// <param name="name">the name of the update request</param>
        /// <param name="location"> the location paired with the name</param>
        /// <param name="address">the address of the server</param>
        /// <param name="port">the port to send the message too</param>
        /// <param name="timeout">the timeout of the request</param>
        /// <param name="protocol">the protocol of the rerquest</param>
        /// <param name="debug">is the client in debug mode</param>
        /// <returns>a string to be passed to wpf</returns>
        public static string WPFInitialise(string name, string location, string address, int port, int timeout, MessageProtocol protocol, bool debug) {
            // phase the arguments in to command line arguments
            string args = "";
            args += name + " ";
            args += location + " ";
            args += "-h " + address + " ";
            args += "-p " + port + " ";
            args += "-t " + timeout + " ";
            // switch the portocols
            switch (protocol) {
                case MessageProtocol.WhoIs:
                    
                    break;
                case MessageProtocol.HTTP1:
                    args += "-h0 ";
                    break;
                case MessageProtocol.HTTP9:
                    args += "-h9 ";
                    break;
                case MessageProtocol.HTTP11:
                    args += "-h1 ";
                    break;
            }
            args += "-d";
            if (debug) {
                Console.WriteLine(args);
            }
            //start the server and return the string
            return start(args.Trim().Split(' '));
            
        }
        /// <summary>
        /// This method is called by wpf to start a lookup request
        /// </summary>
        /// <param name="name">the name to look up</param>
        /// <param name="address">the address of the server</param>
        /// <param name="port"> the port to send the message to</param>
        /// <param name="timeout"> the timeout period for the request</param>
        /// <param name="protocol">the protocol to send the message by</param>
        /// <param name="debug"> if the client is in debug </param>
        /// <returns>a string to be passed to wpf</returns>
        public static string WPFInitialise(string name, string address, int port, int timeout, MessageProtocol protocol, bool debug) {
            // pharse the arguments
            string args = "";
            args += name + " ";
            args += "-h " + address + " ";
            args += "-p " + port + " ";
            args += "-t " + timeout + " ";
            switch (protocol)
            {
                case MessageProtocol.WhoIs:

                    break;
                case MessageProtocol.HTTP1:
                    args += "-h0 ";
                    break;
                case MessageProtocol.HTTP9:
                    args += "-h9 ";
                    break;
                case MessageProtocol.HTTP11:
                    args += "-h1 ";
                    break;
            }
            args += "-d";
            // start the client and return the stirng
            return start(args.Trim().Split(' '));
        }
    }
}
