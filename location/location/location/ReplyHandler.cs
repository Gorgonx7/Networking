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
        string reply = "";
        public ReplyHandler(StreamReader sr, Message pMessage)
        {
            System.Threading.Thread.Sleep(75);
            string holder = Readdata(sr).Trim('\0');
            if (pMessage.GetDebug()) {
                Console.WriteLine(holder);
                Console.WriteLine("----------------------------------------------------------------------------------");
            }
            if (pMessage.GetProtocol() == MessageProtocol.WhoIs)
            {
                try
                {
                   
                    if (holder == "OK\r\n" && pMessage.getType() == MessageType.update)
                    {
                        reply = pMessage.GetName() + " location changed to be " + pMessage.GetLocation();
                        Console.WriteLine(reply);
                    }
                    else if (holder != "ERROR: no entries found\r\n")
                    {
                        reply = pMessage.GetName() + " is " + holder.TrimEnd();
                        Console.WriteLine(reply);
                    }
                    else
                    {
                        reply = holder;
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
                    string LineHolder = holder.Split((char) 13)[0];
                    if (!LineHolder.Split(' ').Contains("404"))
                    {
                        
                        LineHolder = holder.Split((char)13)[4];
                        reply = pMessage.GetName() + " is " + holder.Split((char)13)[3].TrimEnd().TrimStart();
                        Console.WriteLine(reply);
                    }
                    else
                    {
                        reply = "ERROR: no entries found\r\n";
                        Console.Write(reply);
                    }
                }
                else
                {
                    reply = pMessage.GetName() + " location changed to be " + pMessage.GetLocation();
                    Console.WriteLine(reply);
                }
            }



        }
        public string GetReply() {
            return reply;
        }
        private string Readdata(StreamReader sr) {
            Stream stream = sr.BaseStream;
            byte[] buffer = new byte[25];
            long read = 0;

            int chunk;
            while ((chunk = stream.Read(buffer, (int)read, buffer.Length - (int)read)) > 0)
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if (nextByte == -1)
                    {
                        return System.Text.Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                    }

                    // Nope. Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy(buffer, ret, read);
            
            return System.Text.Encoding.ASCII.GetString(buffer);

        }
    }
}
