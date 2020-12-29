using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRestServer
{
    public class Message
    {
        public int ID { get; set; }
        public string Content { get; set; }


        public Message(int id, string content)
        {
            ID = id;
            Content = content;
        }

        public void Print()
        {
            Console.WriteLine(Content);
        }

        public void Update(string newContent)
        {
            Content = newContent;
        }

    }
}
