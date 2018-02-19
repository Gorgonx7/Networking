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
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    pWriter.WriteLine("OK");
                    pWriter.Flush();
                    pWriter.Close();
                    break;
                case Protocol.HTTP1:
                    throw new NotImplementedException();
                    break;
                case Protocol.HTTP11:
                    throw new NotImplementedException();
                    break;
                case Protocol.HTTP9:
                    throw new NotImplementedException();
                    break;
            }
        }
        public static void Locate(StreamWriter pWriter, Protocol pProtocol, string pName) {
            switch (pProtocol)
            {
                case Protocol.WhoIs:
                    pWriter.WriteLine(Program.m_Manager.GetLocation(pName));
                    pWriter.Flush();
                    pWriter.Close();
                    break;
                case Protocol.HTTP1:
                    throw new NotImplementedException();
                    break;
                case Protocol.HTTP11:
                    throw new NotImplementedException();
                    break;
                case Protocol.HTTP9:
                    throw new NotImplementedException();
                    break;
            }
        }

    }
}
