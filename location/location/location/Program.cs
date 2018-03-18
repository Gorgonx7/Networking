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
        public static void Main(string[] args)
        {
            start(args);
            


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
        public static string start(string[] args)
        {
            try
            {
                Message message = new Message(args);
                return MessageWrapper.SendMessage(message);
            }
            catch (Exception e)
            {
                return "Error unknown in settings";
            }
        }
        public static string WPFInitialise(string name, string location, string address, int port, int timeout, MessageProtocol protocol, bool debug) {
            string args = "";
            args += name + " ";
            args += location + " ";
            args += "-h " + address + " ";
            args += "-p " + port + " ";
            args += "-t " + timeout + " ";
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
            return start(args.Trim().Split(' '));
            
        }
        public static string WPFInitialise(string name, string address, int port, int timeout, MessageProtocol protocol, bool debug) {
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

            return start(args.Trim().Split(' '));
        }
    }
}
