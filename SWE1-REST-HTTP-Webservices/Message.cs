using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
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
