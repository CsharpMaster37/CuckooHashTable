using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HashTablesLib;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RealizeHashTable
{
    internal class Program
    {
        public static void TestText(IDictionary<string,int> collection,string[] text)
        {
            IDictionary<string,int> table = collection;
            HashSet<string> keys = new HashSet<string>();
            foreach (var item in text)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, 1);
                }
                else
                {
                    table[item]++;
                    if (table[item] >= 27)
                        keys.Add(item);
                }
            }
            foreach (var item in keys)
            {
                table.Remove(item);
            }
        }
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader("../../WarAndWorld.txt", Encoding.GetEncoding(1251));
            string[] text = sr.ReadLine().Split(new char[] { ' ', ',', '.'}, StringSplitOptions.RemoveEmptyEntries);
            CuckooHashTable<string, int> table = new CuckooHashTable<string, int>();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TestText(table,text);
            sw.Stop();
            Console.WriteLine($"Время хеш-таблицы: {sw.ElapsedMilliseconds}");
            sw.Restart();
            TestText(dict,text);
            sw.Stop();
            Console.WriteLine($"Время словаря: {sw.ElapsedMilliseconds}");
        }
    }
}
