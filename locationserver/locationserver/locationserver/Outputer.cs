﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace locationserverConsole
{
    public enum Type { update, lookup } // the enum for the type of message
    public enum Protocol { HTTP1, HTTP11, HTTP9, WhoIs } // the enum for the protocol
    static class Outputer
    {

        
       
        /// <summary>
        /// This method is called when the message was a update request
        /// </summary>
        /// <param name="pWriter">the stream writer attatched to the client</param>
        /// <param name="pProtocol">the protocol that the message was sent by</param>
        /// <param name="socket">the socket for logging</param>
        /// <param name="counter">the counter for logging</param>
        public static void Update(StreamWriter pWriter, Protocol pProtocol, Socket socket, int counter) {
            string Output = "";
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    Output = "OK";
                    pWriter.WriteLine("OK");
                    
                    break;
                case Protocol.HTTP1:
                    /*
                     * HTTP/1.0<space>200<space>OK<CR><LF>
                       Content-Type:<space>text/plain<CR><LF>
                       <CR><LF>*/
                    Output = "HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n";
                    pWriter.WriteLine(Output);
                    break;
                case Protocol.HTTP11:
                    /*  HTTP/1.1<space>200<space>OK<CR><LF>
                        Content-Type:<space>text/plain<CR><LF>
                        <optional header lines><CR><LF>*/

                    Output = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n";
                    pWriter.WriteLine(Output);
                    break;
                case Protocol.HTTP9:
                    /*
                     *  HTTP/0.9<space>200<space>OK<CR><LF>
                        Content-Type:<space>text/plain<CR><LF>
                        <CR><LF>
                        */
                     Output = "HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n";
                    pWriter.WriteLine(Output);
                    break;
            }
            Console.WriteLine(">> Replying to client with: ");
            Console.Write(Output);
            Log.AddLog("Replying to client " + counter + "With message: " + Output, socket);
            pWriter.Flush();
            
        }
        /// <summary>
        /// This method is the same as the above method except for look ups
        /// </summary>
        /// <param name="pWriter"> the stream writer attatched to the client stream</param>
        /// <param name="pProtocol">the protocol the message was sent by</param>
        /// <param name="pName">the name of the location to look up</param>
        /// <param name="socket">the socket for logging</param>
        /// <param name="counter">the client number for logging</param>
        public static void Locate(StreamWriter pWriter, Protocol pProtocol, string pName, Socket socket, int counter) {
            string output = "";
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    output = Program.m_Manager.GetLocation(pName);
                    pWriter.WriteLine(output);
                   
                    break;
                case Protocol.HTTP1:
                    if (Program.m_Manager.GetLocation(pName) == "ERROR: no entries found")
                    {
                        /*
                         *  HTTP/1.0<space>404<space>Not<space>Found<CR><LF>
                            Content-Type:<space>text/plain<CR><LF>
                            <CR><LF>*/
                        output = "HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n";
                        pWriter.WriteLine(output);
                    }
                    else
                    {
                        /*
                         *  HTTP/1.0<space>200<space>OK<CR><LF>
                            Content-Type:<space>text/plain<CR><LF>
                            <CR><LF>
                            <location><CR><LF>*/
                        output = "HTTP/1.0 200 OK\r\nContent-Type: text/plain\r\n\r\n" + Program.m_Manager.GetLocation(pName);
                        pWriter.WriteLine(output);
                    }
                    break;
                case Protocol.HTTP11:
                    if (Program.m_Manager.GetLocation(pName) == "ERROR: no entries found")
                    {
                        /*
                        HTTP/1.1<space>404<space>Not<space>Found<CR><LF>
                        Content-Type:<space>text/plain<CR><LF>
                        <optional header lines><CR><LF>    
                     */
                        output = "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n";
                        pWriter.WriteLine(output);
                    }
                    else
                    {
                        /*  HTTP/1.1<space>200<space>OK<CR><LF>
                            Content-Type:<space>text/plain<CR><LF>
                            <optional header lines><CR><LF>
                            <location><CR><LF>*/
                        string Holder = Program.m_Manager.GetLocation(pName);
                        output = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n" + Holder;
                        pWriter.WriteLine(output);
                    }
                    break;
                case Protocol.HTTP9:
                    if (Program.m_Manager.GetLocation(pName) == "ERROR: no entries found")
                    {
                        /*
                         *  HTTP/0.9<space>404<space>Not<space>Found<CR><LF>
                            Content-Type:<space>text/plain<CR><LF>
                            <CR><LF>*/
                        output = "HTTP/0.9 404 Not Found\r\nContent-Type: text/plain\r\n";
                        pWriter.WriteLine(output);
                    }
                    else
                    {
                        /*
                         *  HTTP/0.9<space>200<space>OK<CR><LF>
                            Content-Type:<space>text/plain<CR><LF>
                            <CR><LF>
                            <location><CR><LF>*/
                        string holder = Program.m_Manager.GetLocation(pName);
                        output = "HTTP/0.9 200 OK\r\nContent-Type: text/plain\r\n\r\n" + holder;
                        pWriter.WriteLine(output);
                    }
                    break;
            }
            if (Program.m_Debug)
            {
                Console.WriteLine(">> Replying to client with: ");
                Console.Write(output);
                
            }
            Log.AddLog("Replying to client " + counter + "With message: " + output, socket);
            pWriter.Flush();
           
        }

    }
}
