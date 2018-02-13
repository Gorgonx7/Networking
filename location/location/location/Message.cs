using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace location
{
    enum MessageProtocol {HTTP9, HTTP1, HTTP11, WhoIs  }
    enum MessageType { lookup, update }
    class Message
    {
        private MessageType m_Type;
        private MessageProtocol m_Protocol;
        public Message(string[] args) {
            if (args.Contains("HTTP/1.0"))
            {
                m_Protocol = MessageProtocol.HTTP1;
            }
            else if (args.Contains("HTTP/1.1"))
            {
                m_Protocol = MessageProtocol.HTTP11;
            }
            switch (args.Length) {
                case 1:
                    //Not a HTTP 0.9 request just a whois request
                    m_Protocol = MessageProtocol.WhoIs;
                    break;
                case 2:
                    //could be a HTTP 0.9 request as a get or a who is update 
                    break;
                default:
                    break;
            }
        }


    }
}
