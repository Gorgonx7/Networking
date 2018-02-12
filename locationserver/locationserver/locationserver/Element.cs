using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace locationserver
{
    class Element
    {
        string m_Name;
        string m_Location;
        public Element(string pName, string pLocation) {
            m_Name = pName;
            m_Location = pLocation;
        }

        public string getName() {
            return m_Name;
        }
        public string getLocation() {
            return m_Location;
        }
        public bool updateLocation(string pString) {
            m_Location = pString;
            return true;
        }
        public override string ToString()
        {
            return m_Name + ", " + m_Location;
        }
    }
}
