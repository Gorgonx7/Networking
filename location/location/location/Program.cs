﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
namespace location
{
    class Program
    {
        static void Main(string[] args)
        {
            
                try
                {
                    string exception = args[0];
                    TcpClient client = new TcpClient();
                    client.Connect("whois.net.dcs.hull.ac.uk", 43);
                    StreamWriter sw = new StreamWriter(client.GetStream());
                    StreamReader sr = new StreamReader(client.GetStream());

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
            
            
        }
    }
}
