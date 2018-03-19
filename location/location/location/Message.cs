using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace location
{
    public enum MessageProtocol { HTTP9, HTTP1, HTTP11, WhoIs }
    enum MessageType { lookup, update }

    class Message
    {
        private MessageType m_Type;
        private MessageProtocol m_Protocol = MessageProtocol.WhoIs;
        private int m_Port = 43;
        private int m_Timeout = 1000;
        private bool m_Debug = false;
        private string m_Username = "";
        private string m_Location = "";
        private bool m_FoundUsername = false;
        private string m_HostName = "whois.net.dcs.hull.ac.uk";
        public Message(string[] args)
        {
            CheckProtocols(args);
            if (m_Debug) {
                Console.WriteLine("Message string: " + ToString());
            }
        }
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
        private void CheckProtocols(string[] args)
        {

            for (var x = 0; x < args.Length; x++)
            {
                if (args[x] == "")
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

                if (args[x][0] == '-')
                {
                    switch (args[x])
                    {
                        
                        case "-p":
                            m_Port = int.Parse(args[x + 1]);
                            
                            x += 1;
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
                else
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
                }

            }
        }
        
        public override string ToString()
        {
            string Output = "";
            switch (m_Protocol)
            {
                case MessageProtocol.HTTP1:
                    if (m_Type == MessageType.lookup)
                    {
                        Output = "GET /?" + m_Username + " HTTP/1.0\r\n";
                    }
                    else
                    {
                        /*POST<space>/<name><space>HTTP/1.0<CR><LF>
                          Content-Length:<space><length><CR><LF>
                          <optional header lines><CR><LF>
                          <location>*/
                        Output = "POST /" + m_Username + " HTTP/1.0\r\n" + "Content-Length: " + m_Location.Length + "\r\n\r\n" + m_Location;
                    }
                    break;
                case MessageProtocol.HTTP11:
                    if (m_Type == MessageType.lookup)
                    {
                        /*GET<space>/?name =< name >< space > HTTP / 1.1 < CR >< LF >
                          Host :< space >< hostname >< CR >< LF >
                          < optional header lines>< CR >< LF >*/
                        Output = "GET /?name=" + m_Username + " HTTP/1.1\r\n" + "Host: " + m_HostName + "\r\n";
                    }
                    else
                    {
                        /*POST<space>/<space>HTTP/1.1<CR><LF>
                          Host:<space><hostname><CR><LF>
                          Content-Length:<space><length><CR><LF>
                          <optional header lines><CR><LF>
                          name=<name>&location=<location>*/
                        string content = "name=" + m_Username + "&location=" + m_Location;
                        Output = "POST / HTTP/1.1\r\nHost: " + m_HostName + "\r\nContent-Length: " + content.Length + "\r\n\r\n" + "name=" + m_Username + "&location=" + m_Location;
                    }
                    break;
                case MessageProtocol.HTTP9:
                    if (m_Type == MessageType.lookup)
                    {
                        Output = "GET /" + m_Username;
                    }
                    else
                    {
                        Output = "PUT /" + m_Username + "\r\n\r\n" + m_Location;
                    }
                    break;
                case MessageProtocol.WhoIs:
                    if (m_Type == MessageType.lookup)
                    {
                        Output = m_Username;
                    }
                    else
                    {
                        
                        Output = m_Username + " " + m_Location;
                    }
                    break;
            }
            return Output;
        }

    }
}
