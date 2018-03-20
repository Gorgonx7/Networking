using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace locationserverConsole
{
    public class ElementManager
    {
        private Dictionary<string, string> dictionary = new Dictionary<string, string>(); // define the dictionary
        public ElementManager() {
            

        }
        public string GetLocation(string pName) {
            // try to get the value in the dictonary
            string Output = "";
            if (dictionary.TryGetValue(pName, out Output))
            {
                return Output; // if it is found return it
            }
            else
            {
                return "ERROR: no entries found"; // if not return the error message
            }
        }

        public bool UpdateLocation(string pName, string pLocation) {

            //check if the dictionary contains it
            if (dictionary.ContainsKey(pName))
            {
                dictionary[pName] = pLocation; // if it does update
            }
            else {
                dictionary.Add(pName, pLocation); // if not create it
            }
            if (Program.m_Debug)
            {
                Console.WriteLine("Added or changed Entry: " + pName + " At " + pLocation);
            }
            return true;
        }
        
        public void SaveElements(string Path) {
            // try to save each of the elements in the dictionary
            StreamWriter streamWriter = new StreamWriter(Path);
            
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                streamWriter.WriteLine(item.Key.Length + " " + item.Key + " " + item.Value.Length + " " + item.Value); // pharse the data correctly
            }

            streamWriter.Flush();
           
        }
        /// <summary>
        /// loads the data from the specified file location
        /// </summary>
        /// <param name="Path">the path to the location</param>
        public void LoadElements(string Path) {
            
            StreamReader streamReader = new StreamReader(Path);
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
        /// <summary>
        /// returns the dictionary, used in debugging
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetDictionary() {
            return dictionary;
        }
        
    }
}
