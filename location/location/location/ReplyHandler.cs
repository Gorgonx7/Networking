using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace location
{
    class ReplyHandler
    {
        public ReplyHandler(StreamReader sr, Message pMessage)
        {
            if (pMessage.GetProtocol() == MessageProtocol.WhoIs)
            {
                try
                {
                    string holder = sr.ReadToEnd();
                    if (holder == "OK\r\n" && pMessage.getType() == MessageType.update)
                    {
                        Console.WriteLine(pMessage.GetName() + " location changed to be " + pMessage.GetLocation());
                    }
                    else if (holder != "ERROR: no entries found\r\n")
                    {
                        Console.WriteLine(pMessage.GetName() + " is " + holder.TrimEnd());
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
            else
            {
                if (pMessage.getType() == MessageType.lookup)
                {
                    string holder = sr.ReadLine();
                    holder = sr.ReadLine();
                    if (!holder.Split(' ').Contains("404"))
                    {
                        holder = sr.ReadLine();
                        holder = sr.ReadLine();
                        Console.WriteLine(pMessage.GetName() + " is " + holder.TrimEnd());
                    }
                    else
                    {
                        Console.Write("ERROR: no entries found\r\n");
                    }
                }
                else
                {
                    Console.WriteLine(pMessage.GetName() + " location changed to be " + pMessage.GetLocation());
                }
            }



        }
    }
}
