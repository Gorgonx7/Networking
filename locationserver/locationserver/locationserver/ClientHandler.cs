using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace locationserver
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
                int counter = 0;
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
                    Console.WriteLine(dataFromClient[x]);
                }


                Phase(dataFromClient);
                switch (m_Type)
                {
                    case Type.lookup:
                        Outputer.Locate(sw, m_Protocol, m_Name);
                        break;
                    case Type.update:
                        Program.m_Manager.UpdateLocation(m_Name, m_Location);
                        Outputer.Update(sw, m_Protocol);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                m_Socket.Close();
            }

        }
        private void Phase(string[] args)
        {
            if (args[0].Split(' ').Contains("HTTP/1.0") && args[0].Split(' ').Length == 3)
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
                        m_Name = args[0].Split(' ')[1].Trim('/');
                        m_Location = args[3];
                        break;
                    case "GET":
                        m_Type = Type.lookup;
                        m_Name = args[0].Split(' ')[1].Trim('/').Trim('?');
                        break;
                    default:
                        throw new Exception("Unexpected protocol message registered as 1.1");

                }
            }
            else if (args[0].Split(' ').Contains("HTTP/1.1") && args[0].Split(' ').Length == 3)
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
                        m_Name = args[4].Split('=')[1].Split('&')[0];
                        m_Location = args[4].Split('=')[2];
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
            else if (args[0].Split(' ')[0] == "GET" && args[0].Split(' ')[1][0] == '/' || args[0].Split(' ')[0] == "PUT" && args[0].Split(' ')[1][0] == '/')
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
                else {
                    /*PUT<space>/<name><CR><LF>
                      <CR><LF>
                      <location><CR><LF>*/
                    m_Type = Type.update;
                    m_Name = args[0].Split(' ')[1].Trim('/');
                    m_Location = args[2];
                }
            }
            else
            {
                m_Protocol = Protocol.WhoIs;
                if (args[0].Split(' ').Length >= 2)
                {
                    m_Type = Type.update;
                    m_Name = args[0].Split(' ')[0];
                    for(int x = 1; x < args[0].Split(' ').Length; x++)
                    {
                        m_Location = m_Location + " " + args[0].Split(' ')[x];
                    }
                    m_Location = m_Location.TrimStart();
                }
                else
                {
                    m_Type = Type.lookup;
                    m_Name = args[0];
                }
            }
        }
    }

}

