using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Mtcg
{
    public class Logger
    {
        public string Title { get; set; }
        public List<string> Log { get; set; }

        public Logger(string name)
        {
            Title = name;
            Log = new List<string>();
            AddEntry(name);
        }

        public void AddEntry(string sentence)
        {
            var entry = new StringBuilder();
            entry.Append(sentence);
            Log.Add(entry.ToString());
        }

        public void Show()
        {
            Console.WriteLine(Title + "\n");
            foreach (var entry in Log)
            {
                Console.WriteLine(entry);
            }

            Console.WriteLine();
        }
    }
}