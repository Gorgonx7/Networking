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
        
        static int m_Port = 43;
        static string m_Address = GetLocalIPAddress();
        public static string SendMessage(Message message)
        {
            ChangePort(message.GetPort());
            ChangeAddress(message.GetAddress());
            return WrapMessage(message);
        }
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
        private static string WrapMessage( Message pMessage)
        {
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(m_Address, m_Port);
            }
            catch {
                return "Failed to connect to server";
                
            }
            if (pMessage.GetTimeout() != 0)
            {
                client.GetStream().ReadTimeout = pMessage.GetTimeout();
            }
            StreamWriter sw = new StreamWriter(client.GetStream());
            StreamReader sr = new StreamReader(client.GetStream());


            try
            {
                string holder = pMessage.ToString();              
                sw.WriteLine(holder);

                sw.Flush();
                System.Threading.Thread.Sleep(75);
                ReplyHandler reply = new ReplyHandler(sr, pMessage);
                return reply.GetReply();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
            finally {
                sw.Dispose();
                sr.Dispose();
            }
        }
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
