using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace location
{

    static class MessageWrapper
    {
        
        static int m_Port = 43; // the default port to send the message to the server is 43
        static string m_Address = GetLocalIPAddress(); // get the local ip address and set it to the default address, this will be overwritten if the message will not be sent to the localhost 
        public static string SendMessage(Message message)
        {
            ChangePort(message.GetPort()); //change the port to the one specified in the message
            ChangeAddress(message.GetAddress()); // change the address to the one specified in the message
            return WrapMessage(message); // return the string upwared for access by wpf
        }
        #region getters and setters
        private static void ChangePort(int pPort)
        {
            m_Port = pPort;
        }
        private static void ChangeAddress(string pAddress)
        {
            if(pAddress == "localhost")
            {
                m_Address = GetLocalIPAddress();
            }
            m_Address = pAddress;
        }
        #endregion

        /// <summary>
        /// wrap the message to send to the server
        /// </summary>
        /// <param name="pMessage"> the message to be sent to the server</param>
        /// <returns> a string to be sent to wpf </returns>
        private static string WrapMessage( Message pMessage)
        {
            TcpClient client = new TcpClient(); // start the client
            try
            {
                client.Connect(m_Address, m_Port); // try to connect to the server
            }
            catch {
                return "Failed to connect to server"; // return upward if the server cannot be connected too
                
            }
            if (pMessage.GetTimeout() != 0) // if the timeout is 0, disable timeouts else
            {
                client.GetStream().ReadTimeout = pMessage.GetTimeout(); // set the timeout to the one specified in the message
            }
            StreamWriter sw = new StreamWriter(client.GetStream()); // attatched a stream writer to the network stream
            StreamReader sr = new StreamReader(client.GetStream()); // attatched a stream reader to the network stream


            try
            {
                string holder = pMessage.ToString(); // create a holder to store the message in
                sw.WriteLine(holder); // write the message to the stream buffer

                sw.Flush(); // flush the buffer and send the message
                System.Threading.Thread.Sleep(75); // wait 75ms for the server to respond
                ReplyHandler reply = new ReplyHandler(sr, pMessage); // create a reply handler to process the reply from the server
                return reply.GetReply(); // return the reply upwards to the wpf interface

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); // catch the error messages
                return e.Message;
            }
            finally {
                //cleanup
                sw.Dispose();
                sr.Dispose();
            }
        }
        /// <summary>
        /// this method gets the local IP address, it is/was used for debugging
        /// </summary>
        /// <returns>the local ip address</returns>
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
