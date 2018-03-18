using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace locationserverConsole
{
    class ClientHandler
    {
        Socket m_Socket;
        int m_ClientNumber;
        Protocol m_Protocol;
        Type m_Type;
        string m_Name;
        string m_Location;
        public ClientHandler()
        {

        }

        public void startClient(Socket pClientSocket, int pClientNumber)
        {
            m_Socket = pClientSocket;
            m_ClientNumber = pClientNumber;
            Log.AddLog("Accepting client" + m_ClientNumber + "OK", pClientSocket);
            Thread Thread = new Thread(DoRequest);
            Thread.Start();
        }
        private void DoRequest()
        {
            int requestCount = 0;

            string[] dataFromClient;
            requestCount = 0;


            try
            {
                requestCount = requestCount + 1;
                NetworkStream networkStream = new NetworkStream(m_Socket);
                networkStream.ReadTimeout = 1000;
                networkStream.WriteTimeout = 1000;
                StreamReader sr = new StreamReader(networkStream);
                StreamWriter sw = new StreamWriter(networkStream);
                
                Console.WriteLine(" >> " + "From client-" + m_ClientNumber);
                List<string> data = new List<string>();
                
                string holder = "";
                while (sr.Peek() > -1)
                {
                    if (sr.Peek() == 13)
                    {
                        data.Add(holder);
                        holder = "";
                        sr.Read();
                        sr.Read();
                        
                    }
                    else {
                        holder += (char)sr.Read();
                    }
                    
                }

               
                
                dataFromClient = new string[data.Count];
                for (int x = 0; x < data.Count; x++)
                {
                    dataFromClient[x] = data[x];
                    if (Program.m_Debug)
                    {
                        Console.WriteLine(dataFromClient[x]);
                    }
           
                }
                Log.AddLog("Recived data from client " + m_ClientNumber + "Data: " + dataFromClient, m_Socket);

                if (!Phase(dataFromClient))
                {
                    m_Protocol = Protocol.WhoIs;
                    if (dataFromClient[0].Split(' ').Length >= 2)
                    {
                        m_Type = Type.update;
                        m_Name = dataFromClient[0].Split(' ')[0];
                        for (int x = 0; x < dataFromClient[0].Split(' ').Length; x++)
                        {
                            m_Location = m_Location + " " + dataFromClient[0].Split(' ')[x];
                        }
                        m_Location = m_Location.TrimStart();
                        if (m_Location[0] == '\t') {
                            m_Location = m_Location.Substring(1);
                            Log.AddLog("Detected ambiguaty between 0.9 and whois protocol, fixing", m_Socket);
                            if (Program.m_Debug)
                            {
                                Console.WriteLine("Detected ambiguaty between 0.9 and whois protocol, fixing");
                            }
                        }
                    }
                    else
                    {
                        m_Type = Type.lookup;
                        m_Name = dataFromClient[0];
                    }
                }


                switch (m_Type)
                {
                    case Type.lookup:
                        Outputer.Locate(sw, m_Protocol, m_Name, m_Socket, m_ClientNumber);
                        break;
                    case Type.update:
                        Program.m_Manager.UpdateLocation(m_Name, m_Location);
                        Outputer.Update(sw, m_Protocol, m_Socket, m_ClientNumber);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.AddLog("Error with Client " + m_ClientNumber + " Error Message: " + ex.Message, m_Socket);
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                m_Socket.Close();
            }

        }
        private bool Phase(string[] args)
        {
            string holderVersion = args[0];
            string version = "";
            try
            {
                version = holderVersion.Substring(holderVersion.Length - 9, 9).Trim();
            }
            catch {
                version = "";
            }
            if (version == "HTTP/1.0")
            {
                /*  GET<space>/?<name><space>HTTP/1.0<CR><LF>
                    <optional header lines><CR><LF>*/

                /*
                    POST<space>/<name><space>HTTP/1.0<CR><LF>
                    Content-Length:<space><length><CR><LF>
                    <optional header lines><CR><LF>
                    <location>

                 */
                m_Protocol = Protocol.HTTP1;
                string[] holder = args[0].Split(' ');
                switch (holder[0])
                {
                    case "POST":
                        m_Type = Type.update;
                        
                        for (int x = 6; x < args[0].Length; x++)
                        {
                            if (args[0][x] == 13)
                            {
                                break;
                            }
                            m_Name += args[0][x];
                        }
                        m_Name = m_Name.Substring(0, m_Name.Length - 9);
                        
                        m_Location = args[3];
                        break;
                    case "GET":
                        m_Type = Type.lookup;
                        for (int x = 6; x < args[0].Length; x++)
                        {
                            if (args[0][x] == 13)
                            {
                                break;
                            }
                            m_Name += args[0][x];
                        }
                        m_Name = m_Name.Substring(0, m_Name.Length - 9);
                        break;
                    default:
                        throw new Exception("Unexpected protocol message registered as 1.1");

                }
            }
            else if (version == "HTTP/1.1")
            {

                m_Protocol = Protocol.HTTP11;
                /*
                    GET<space>/?name=<name><space>HTTP/1.1<CR><LF>
                    Host:<space><hostname><CR><LF>
                    <optional header lines><CR><LF>
                    
                
                POST<space>/<space>HTTP/1.1<CR><LF>
                Host:<space><hostname><CR><LF>
                Content-Length:<space><length><CR><LF>
                <optional header lines><CR><LF>
                name=<name>&location=<location>

                 */
                string[] holder = args[0].Split(' ');
                switch (holder[0])
                {
                    case "POST":
                        m_Type = Type.update;
                       /* int last = 0;
                        string[] lastLocation = args[4].Split('=');
                        for (int x = 0; x < lastLocation.Length; x++) {
                            if (lastLocation[x].Substring(lastLocation[x].Length - 9, 9) == "&location")
                            {
                                last = x;
                                // fix this
                            }
                        }*/
                        for (int x = 5; x < args[4].Length; x++) {
                            if (args[4][x] == '&' && args[4][x+1] == 'l')
                            {
                                break;
                            }
                            else {
                                m_Name += args[4][x];
                            }

                        }
                        for (int x = 15 + m_Name.Length; x < args[4].Length; x++) {
                            if (args[4][x] == 13) {
                                break;
                            }
                            m_Location += args[4][x];
                        }
                        break;
                    case "GET":
                        m_Type = Type.lookup;
                        string Holder = "";
                        for (int index = "GET /?name=".Length; index < args[0].Length; index++)
                        {
                            if ((int)args[0][index] == 13)
                            {
                                break;
                            }
                            else
                            {
                                Holder += args[0][index];
                            }
                        }
                        m_Name = Holder.Substring(0, Holder.Length - "HTTP/1.1".Length).Trim();
                        break;
                    default:
                        throw new Exception("Unexpected protocol message registered as 1.1");
                        
                }
                
            }
            else
            {
                try
                {
                    if (args[0].Split(' ')[0] == "GET" && args[0].Split(' ')[1][0] == '/' || args[0].Split(' ')[0] == "PUT" && args[0].Split(' ')[1][0] == '/')
                    {
                        m_Protocol = Protocol.HTTP9;
                        if (args[0].Split(' ')[0] == "GET")
                        {
                            /*GET<space>/<name><CR><LF>*/
                            m_Type = Type.lookup;
                            string Holder = "";
                            for (int index = 5; index < args[0].Length; index++)
                            {
                                if ((int)args[0][index] == 13)
                                {
                                    break;
                                }
                                else
                                {
                                    Holder += args[0][index];
                                }
                            }
                            m_Name = Holder;
                        }
                        else
                        {
                            /*PUT<space>/<name><CR><LF>
                              <CR><LF>
                              <location><CR><LF>*/
                            m_Type = Type.update;
                            m_Name = "";
                            for (int x = 5; x < args[0].Length; x++)
                            {
                                if (args[0][x] == 13)
                                {
                                    break;
                                }
                                m_Name += args[0][x];
                            }
                            m_Location = args[2];
                        }

                    }
                    else {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
                
                
            }
            return true;
        }
       
    }
    
}

