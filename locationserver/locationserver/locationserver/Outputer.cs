using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace locationserver
{
    public enum Type { update, lookup }
    public enum Protocol { HTTP1, HTTP11, HTTP9, WhoIs }
    static class Outputer
    {

        
       

        public static void Update(StreamWriter pWriter, Protocol pProtocol) {
            string Output = "";
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    pWriter.WriteLine("OK");
                    pWriter.Flush();
                    pWriter.Close();
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
            pWriter.Flush();
            pWriter.Close();
        }
        public static void Locate(StreamWriter pWriter, Protocol pProtocol, string pName) {
            string output = "";
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    pWriter.WriteLine(Program.m_Manager.GetLocation(pName));
                   
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
            Console.WriteLine(">> Replying to client with: ");
            Console.Write(output);
            pWriter.Flush();
            pWriter.Close();
        }

    }
}
