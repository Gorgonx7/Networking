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
        TcpClient clientSocket;
        string clNo;
        Protocol m_Protocol;
        Type m_Type;
        string m_Name;
        string m_Location;
        public ClientHandler() {

        }
        
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread Thread = new Thread(DoRequest);
            Thread.Start();
        }
        private void DoRequest() {
            int requestCount = 0;

            string[] dataFromClient;
            requestCount = 0;

            while (true)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    StreamReader sr = new StreamReader(networkStream);
                    StreamWriter sw = new StreamWriter(networkStream);
                    
                    Console.WriteLine(" >> " + "From client-" + clNo);
                    List<string> data = new List<string>();
                    while (!sr.EndOfStream) {
                        data.Add(sr.ReadLine());
                    }
                    dataFromClient = new string[data.Count];
                    for (int x = 0; x < data.Count; x++) {
                        Console.WriteLine(dataFromClient[x]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine( ex.ToString());
                }
            }
        }
        private void Phase(string[] args)
        {
            if (args.Contains("HTTP/1.0"))
            {
                m_Protocol = Protocol.HTTP1;
            }
            else if (args.Contains("HTTP/1.1"))
            {
                m_Protocol = Protocol.HTTP11;
            }
            else if (args[0].Split(' ')[0] == "GET" && args[0].Split(' ').Length == 2 || args[0] == "PUT" && args.Length == 3)
            {
                m_Protocol = Protocol.HTTP9;
            }
            else {
                if (args[0].Split(' ').Length == 2)
                {
                    m_Type = Type.update;
                    m_Name = args[0].Split(' ')[0];
                    m_Location = args[0].Split(' ')[1];
                }
                else {
                    m_Type = Type.lookup;
                    m_Name = args[0];
                }
            }
        }
        }

    }
}
