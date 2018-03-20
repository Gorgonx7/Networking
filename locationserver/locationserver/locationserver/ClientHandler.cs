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
        Socket m_Socket; // the current client socket
        int m_ClientNumber; // the current client number
        Protocol m_Protocol; // the current client protocol being used
        Type m_Type; // the current type of message being processed
        string m_Name; // the name of the request/update
        string m_Location; // the location of the update
        public ClientHandler()
        {

        }

        public void startClient(Socket pClientSocket, int pClientNumber)
        {
            m_Socket = pClientSocket; // assign the socket
            m_ClientNumber = pClientNumber; // assign the client number
            Log.AddLog("Accepting client" + m_ClientNumber + " OK", pClientSocket); // update the log 
            Thread Thread = new Thread(DoRequest); // start a new thread to process the request
            Thread.Start(); // start the request
        }
        private void DoRequest()
        {
            

            string[] dataFromClient; // store the data from the client
            


            try
            {
                
                NetworkStream networkStream = new NetworkStream(m_Socket); // create and attatch the network stream
                networkStream.ReadTimeout = Program.m_Timeout; // set the timeouts
                networkStream.WriteTimeout = Program.m_Timeout;
                StreamReader sr = new StreamReader(networkStream);
                StreamWriter sw = new StreamWriter(networkStream);
                if (Program.m_Debug)
                {
                    Console.WriteLine("Timeout = " + Program.m_Timeout);
                    Console.WriteLine(" >> " + "From client-" + m_ClientNumber);
                }

                List<string> data = new List<string>();
                //read in the data from the client in bytes into a list
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

               
                // phase that data into a static array 
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
                //phase the data from the client
                if (!Phase(dataFromClient))
                {
                    // if it is not a HTTP request it's a whois request
                    m_Protocol = Protocol.WhoIs;
                    //get the data from the client and then add that to the two respective strings
                    if (dataFromClient[0].Split(' ').Length >= 2) 
                    {
                        m_Type = Type.update;
                        m_Name = dataFromClient[0].Split(' ')[0];
                        for (int x = 1; x < dataFromClient[0].Split(' ').Length; x++)
                        {
                            m_Location += " " + dataFromClient[0].Split(' ')[x];
                        }
                        m_Location = m_Location.TrimStart();
                        
                    }
                    else
                    {
                        m_Type = Type.lookup;
                        m_Name = dataFromClient[0]; // if it's a look up just take the name
                    }
                }

                // switch the type of message to be sent
                switch (m_Type)
                {
                    case Type.lookup:
                        Outputer.Locate(sw, m_Protocol, m_Name, m_Socket, m_ClientNumber); // call the respective output method
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
                m_Socket.Close(); // tidy up
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>if it is a HTTP request return true, if it is a who is request return false</returns>
        private bool Phase(string[] args)
        {
            string holderVersion = args[0];
            string version = "";
            try
            {
                version = holderVersion.Substring(holderVersion.Length - 9, 9).Trim(); // see if it a http/1.0+ request
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
                // determine what type of request it is and extract the data
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
                // determine what type of request it is and extract the data
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
                    // if it is a http/0.9 request check for it
                    if (args[0].Split(' ')[0] == "GET" && args[0].Split(' ')[1][0] == '/' || args[0].Split(' ')[0] == "PUT" && args[0].Split(' ')[1][0] == '/')
                    {
                        m_Protocol = Protocol.HTTP9;
                        if (args[0].Split(' ')[0] == "GET") // define the type of request
                        {
                            /*GET<space>/<name><CR><LF>*/
                            // extract the data
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
                        return false; // it must be a who is request
                    }
                }
                catch
                {
                    return false; // must be a whois request
                }
                
                
            }
            return true; // the appropreate settings have been found and it is a http request
        }
       
    }
    
}

