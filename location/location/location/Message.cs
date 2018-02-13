using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace location
{
    enum MessageProtocol {HTTP9, HTTP1,  }
    enum MessageType { lookup, update }
    class Message
    {

        public Message(string[] args) {
            if (args.Contains("HTTP/1.0"))
            {

            }
            else if (args.Contains("HTTP/1.1"))
            {

            }
            else if (args.Length == 2 && args.Contains("GET")) {

            }
            switch (args.Length) {
                case 1:

                    break;
                case 2:

                    break;
                default:
                    break;
            }
        }


    }
}
