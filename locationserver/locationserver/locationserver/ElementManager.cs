using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace locationserver
{
    class ElementManager
    {
        private List<Element> m_Elements = new List<Element>();
        public ElementManager() {
            LoadElements(ref m_Elements);

        }
        public string GetLocation(string pName) {
            foreach (Element e in m_Elements) {
                if (e.getName() == pName) {
                    return e.getLocation();
                }
            }
            return "ERROR: no entries found";
        }

        public bool UpdateLocation(string pName, string pLocation) {
            foreach (Element e in m_Elements) {
                if (e.getName() == pName) {
                    return e.updateLocation(pLocation);
                }
            }
            m_Elements.Add(new Element(pName, pLocation));
            Console.WriteLine("Added New Entry: " + pName + " At " + pLocation);
            return true;
        }
        public void SaveElements() {
            StreamWriter sw = new StreamWriter("database.txt");
            foreach (Element e in m_Elements) {
                sw.WriteLine(e.ToString());
            }
            sw.Flush();
            sw.Close();
        }
        private void LoadElements(ref List<Element> pElements) {
            try
            {
                StreamReader sr = new StreamReader("database.txt");
                while (!sr.EndOfStream)
                {
                    string Element = sr.ReadLine();
                    string[] SplitElement = Element.Split(',');
                    pElements.Add(new Element(SplitElement[0], SplitElement[1]));
                }
                sr.Close();
            }
            catch {

            }
        }


    }
}
