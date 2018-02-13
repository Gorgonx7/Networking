using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace location
{
    
    static class MessageWrapper
    {
        static int m_Port = 43;
        static string m_Address = GetLocalIPAddress();

        public static void ChangePort(int pPort) {
            m_Port = pPort;
        }
        public static void ChangeAddress(string pAddress) {
            m_Address = pAddress;
        }
        public static string WrapMessage(string pName, string pLocation) {
            string Output = "";
            return Output;
        }
        public static string WrapMessage(string pName) {
            string Output = "";
            return Output;
        }

        public static string GetLocalIPAddress()
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
