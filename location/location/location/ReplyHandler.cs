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
        /// <summary>
        /// The constructor to reply handler that process the reply from the serrver
        /// </summary>
        /// <param name="sr">the stream reader connected to the server</param>
        /// <param name="pMessage">the message that was sent to the server</param>
        public ReplyHandler(StreamReader sr, Message pMessage)
        {
            
            string holder = Readdata(sr, pMessage.GetDebug()).Trim('\0'); // the holder from the data holds the data read from the server
            if (pMessage.GetDebug()) {
                Console.WriteLine(holder);
                Console.WriteLine("----------------------------------------------------------------------------------");
            }
            //phase the reply based on the protocol
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
                        // this handles html requests by looking for the doctype tag on line 14
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
                            goto JumpOut;// jump out as too not hit the rest of the request
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
        /// <summary>
        /// returns the reply
        /// </summary>
        /// <returns>returns the string to be passed to wpf</returns>
        public string GetReply() {
            return reply;
        }
        /// <summary>
        /// reads the data in bytes from the server
        /// </summary>
        /// <param name="sr">the stream reader tied to the network</param>
        /// <param name="debug">if the server is in debug mode</param>
        /// <returns>the string containing the data</returns>
        private string Readdata(StreamReader sr, bool debug) {
            Stream stream = sr.BaseStream; // gets the base stream from the stream reader
            byte[] buffer = new byte[25]; // creates the buffer object
            long read = 0; // the current chunk that is read in

            int chunk; // the current chunk
            try
            {
                
                while ((chunk = stream.Read(buffer, (int)read, buffer.Length - (int)read)) > 0) // reads the data in byte form, this is interesting as it reads all the data quickly
                {
                    read += chunk; // add the data to the read

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
                // this is triggered when the timeout has expired or if any exception is thrown
                        byte[] ret = new byte[read]; // resize the buffer
                        Array.Copy(buffer, ret, read);
                if (debug) {
                    Console.WriteLine("Client timed out, consider extending the timeout period if you require more data");
                }
                try
                {
                    if (System.Text.Encoding.ASCII.GetString(buffer).Split((char)13)[14].Contains("<!DOCTYPE html>")) // if it's a web server request
                    {
                        // return any data read in
                        return System.Text.Encoding.ASCII.GetString(buffer);
                    }
                }
                catch {

                }
                if(buffer.Length == 0)
                {
                    throw e; // if the buffer is 0 throw an error
                }
                return System.Text.Encoding.ASCII.GetString(buffer); // return any data readin if when have read in data
                
            }
        }
    }
}
