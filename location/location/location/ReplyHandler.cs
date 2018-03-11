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
                   string holder = Readdata(sr);
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
                        return System.Text.Encoding.ASCII.GetString(buffer);
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
