using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace location
{
    public enum MessageProtocol { HTTP9, HTTP1, HTTP11, WhoIs } // these are all the types of protocol that are implemented
    enum MessageType { lookup, update } // these are the types of message that can be used

    class Message
    {
        private MessageType m_Type; // this is the type of message that is to be sent
        private MessageProtocol m_Protocol = MessageProtocol.WhoIs; // the default protocol is whois
        private int m_Port = 43; // the default port is 43
        private int m_Timeout = 1000; // the default timeout is 1 second, it is measured in ms
        private bool m_Debug = false; // the default debug mode is deactivated
        private string m_Username = ""; // the holder for the username
        private string m_Location = ""; // the holder for the location to be added to the database
        private bool m_FoundUsername = false; // if we have found the username in the commandline arguments
        private string m_HostName = "whois.net.dcs.hull.ac.uk"; // the default host name
        public Message(string[] args)
        {
            CheckProtocols(args); // Process the args to create the message
            if (m_Debug) {
                Console.WriteLine("Message string: " + ToString());
            }
        }
        #region getters and setters
        public MessageType getType() {
            return m_Type;
        }
        public string GetName() {
            return m_Username;
        }
        public int GetTimeout() {
            return m_Timeout;
        }
        public string GetLocation() {
            return m_Location;
        }
        public MessageProtocol GetProtocol() {
            return m_Protocol;
        }
        public int GetPort() {
            return m_Port;
        }
        public string GetAddress()
        {
            return m_HostName;
        }
        public bool GetDebug()
        {
            return m_Debug;
        }
        #endregion


        private void CheckProtocols(string[] args)
        {

            for (var x = 0; x < args.Length; x++)
            {
                if (args[x] == "") // handles a null string
                {
                    if (!m_FoundUsername)
                    {
                        m_Type = MessageType.lookup;
                        m_FoundUsername = true;
                        m_Username = args[x];
                    }
                    else
                    {
                        m_Type = MessageType.update;
                        m_Location = args[x];
                    }
                    continue;
                }
                //switch the starter arguments that define specifics
                if (args[x][0] == '-')
                {
                    switch (args[x])
                    {
                        
                        case "-p":
                            m_Port = int.Parse(args[x + 1]);
                            
                            x += 1; // when we find a argument that takes two arguments we increment the counter
                            break;
                        case "-h1":
                            m_Protocol = MessageProtocol.HTTP11;
                            break;
                        case "-h0":
                            m_Protocol = MessageProtocol.HTTP1;
                            break;
                        case "-h9":
                            m_Protocol = MessageProtocol.HTTP9;
                            break;
                        case "-t":
                            m_Timeout = int.Parse(args[x + 1]);
                            x += 1;
                            break;
                        case "-d":
                            m_Debug = true;
                            break;
                        case "-h":
                            
                            
                                m_HostName = args[x + 1];
                            
                            
                            x += 1; 
                            break;
                        default:
                            if (!m_FoundUsername)
                            {
                                m_Type = MessageType.lookup;
                                m_FoundUsername = true;
                                m_Username = args[x];
                            }
                            else
                            {
                                m_Type = MessageType.update;
                                m_Location = args[x];
                            }
                            break;
                    }
                }
                else // if it does not start with the '-' character, 
                {
                    if (!m_FoundUsername) // if we have not found the username then it must be the username
                    {
                        m_Type = MessageType.lookup;
                        m_FoundUsername = true;
                        m_Username = args[x];
                    }
                    else
                    {
                        m_Type = MessageType.update;
                        m_Location = args[x]; // if we have found the username then the message type is update
                    }
                }

            }
        }
        
        public override string ToString()
        {
            string Output = ""; // create the output that is used in tostring
            switch (m_Protocol) // switch the protocol
            {
                case MessageProtocol.HTTP1:
                    if (m_Type == MessageType.lookup)
                    {
                        Output = "GET /?" + m_Username + " HTTP/1.0\r\n"; // format the lookup http/1.0 protocol
                    }
                    else
                    {
                        /*POST<space>/<name><space>HTTP/1.0<CR><LF>
                          Content-Length:<space><length><CR><LF>
                          <optional header lines><CR><LF>
                          <location>*/
                        // format the update for http/1.0 protcol
                        Output = "POST /" + m_Username + " HTTP/1.0\r\n" + "Content-Length: " + m_Location.Length + "\r\n\r\n" + m_Location;
                    }
                    break;
                case MessageProtocol.HTTP11:
                    if (m_Type == MessageType.lookup)
                    {
                        /*GET<space>/?name =< name >< space > HTTP / 1.1 < CR >< LF >
                          Host :< space >< hostname >< CR >< LF >
                          < optional header lines>< CR >< LF >*/
                          // format the lookup for the http/1.1 protocol
                        Output = "GET /?name=" + m_Username + " HTTP/1.1\r\n" + "Host: " + m_HostName + "\r\n";
                    }
                    else
                    {
                        /*POST<space>/<space>HTTP/1.1<CR><LF>
                          Host:<space><hostname><CR><LF>
                          Content-Length:<space><length><CR><LF>
                          <optional header lines><CR><LF>
                          name=<name>&location=<location>*/
                          // format the update for the http/1.1 protocol
                        string content = "name=" + m_Username + "&location=" + m_Location;
                        Output = "POST / HTTP/1.1\r\nHost: " + m_HostName + "\r\nContent-Length: " + content.Length + "\r\n\r\n" + "name=" + m_Username + "&location=" + m_Location;
                    }
                    break;
                case MessageProtocol.HTTP9:
                    if (m_Type == MessageType.lookup)
                    {
                        // format the  lookup for the http/0.9 protocol
                        Output = "GET /" + m_Username;
                    }
                    else
                    {
                        // format the update for the http/0.9 protocol
                        Output = "PUT /" + m_Username + "\r\n\r\n" + m_Location;
                    }
                    break;
                case MessageProtocol.WhoIs:
                    if (m_Type == MessageType.lookup)
                    {
                        //format the whois lookup protocol
                        Output = m_Username;
                    }
                    else
                    {
                        //format the whois update protocol
                        Output = m_Username + " " + m_Location;
                    }
                    break;
            }
            return Output; // return the message to be sent to the server
        }

    }
}
