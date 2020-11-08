using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class Status
    {
        public int Code { get; set; } // later: ENUM for status codes?
        public string Text { get; set; }

        public Status(int code, string text)
        {
            Code = code;
            Text = text;
        }

        public void Print()
        {
            Console.WriteLine("{0} {1}", Code, Text);
        }


    }
}
