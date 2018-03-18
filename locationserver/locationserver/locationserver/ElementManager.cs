using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace locationserver
{
    public class ElementManager
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public ElementManager() {
            LoadElements();

        }
        public string GetLocation(string pName) {
            string Output = "";
            if (dictionary.TryGetValue(pName, out Output))
            {
                return Output;
            }
            else
            {
                return "ERROR: no entries found";
            }
        }

        public bool UpdateLocation(string pName, string pLocation) {

            if (dictionary.ContainsKey(pName))
            {
                dictionary[pName] = pLocation;
            }
            else {
                dictionary.Add(pName, pLocation);
            }
            Console.WriteLine("Added or changed Entry: " + pName + " At " + pLocation);
            return true;
        }
        public void SaveElements(bool sig, string Path) {
            StreamWriter streamWriter = new StreamWriter(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\" + Path);
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                 streamWriter.WriteLine( item.Key.Length + " " + item.Key + " " + item.Value.Length + " " + item.Value);
            }

            streamWriter.Flush();
            streamWriter.Close();
        }
        public void SaveElements(string Path) {
            StreamWriter streamWriter = new StreamWriter(Path);
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                streamWriter.WriteLine(item.Key.Length + " " + item.Key + " " + item.Value.Length + " " + item.Value);
            }

            streamWriter.Flush();
            streamWriter.Close();
        }
        private void LoadElements() {
            StreamReader streamReader = new StreamReader(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\" + @"dictionary.txt");
            while (!streamReader.EndOfStream) {
                string input = streamReader.ReadLine();
                string key = "";
                string value = "";
                int keyLength = int.Parse(input.Trim().Split(' ')[0]);
                for (int x = keyLength.ToString().Length + 1; x < keyLength + keyLength.ToString().Length + 1; x++) {
                    key += input[x];
                }
                // use the information above to find when the second part of the sequence starts
                int valueLength = int.Parse(input.Substring(keyLength + keyLength.ToString().Length + 2).Split(' ')[0]);
                value = input.Substring(keyLength + keyLength.ToString().Length + valueLength.ToString().Length + 3);
                dictionary.Add(key, value);
            }
            streamReader.Close();
        }
        public Dictionary<string, string> GetDictionary() {
            return dictionary;
        }
        
    }
}
