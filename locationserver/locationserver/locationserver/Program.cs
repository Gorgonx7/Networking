using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace locationserver
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener;
            Socket connection;
            NetworkStream socketStream;
            try
            {
                listener = new TcpListener(43);
                listener.Start();
                while (true)
                {
                    
                    connection = listener.AcceptSocket();
                    socketStream = new NetworkStream(connection);
                    
                    socketStream.Close();
                    connection.Close();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);

            }
        }
    }
}
