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
            
            string holder = Readdata(sr, pMessage.GetDebug()).Trim('\0');
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
                        reply = pMessage.GetName() + " location changed to be " + pMessage.GetLocation().Trim();
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
                    try
                    {
                        if (holder.Split((char)13)[14].Contains("<!DOCTYPE html>")) {
                            string[] HTMLholder = holder.Split((char)13);
                            bool first = true;
                            for (int x = 14; x < HTMLholder.Length; x++) {
                                if (first)
                                {
                                    reply += pMessage.GetName() + " is " + HTMLholder[x];
                                    Console.WriteLine(pMessage.GetName() + " is " + HTMLholder[x].Substring(1));
                                }
                                else
                                {
                                    reply += HTMLholder[x];
                                    Console.WriteLine(HTMLholder[x]);
                                }
                            }
                            goto JumpOut;
                        }
                    }
                    catch {

                    }
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
                    JumpOut:;
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
        private string Readdata(StreamReader sr, bool debug) {
            Stream stream = sr.BaseStream;
            byte[] buffer = new byte[25];
            long read = 0;

            int chunk;
            try
            {
                
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
            catch (Exception e)
            {
                        byte[] ret = new byte[read];
                        Array.Copy(buffer, ret, read);
                if (debug) {
                    Console.WriteLine("Client timed out, consider extending the timeout period if you require more data");
                }
                try
                {
                    if (System.Text.Encoding.ASCII.GetString(buffer).Split((char)13)[14].Contains("<!DOCTYPE html>"))
                    {
                        
                        return System.Text.Encoding.ASCII.GetString(buffer);
                    }
                }
                catch {

                }
                if(buffer.Length == 0)
                {
                    throw e;
                }
                return System.Text.Encoding.ASCII.GetString(buffer);
                
            }
        }
    }
}
