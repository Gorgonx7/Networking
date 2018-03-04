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
    class Program
    {
        static void Main(string[] args)
        {

            Message message = new Message(args);
            MessageWrapper.SendMessage(message);


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
    }
}
