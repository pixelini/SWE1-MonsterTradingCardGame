using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class Response
    {
        public int Status { get; set; }
        public string StatusMessage { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }


        public void Print()
        {
            Console.WriteLine("{0} {1}", Status, StatusMessage);
        }


    }
}


