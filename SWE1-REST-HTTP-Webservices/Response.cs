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

        public Response(int status, string statusMessage)
        {
            Status = status;
            StatusMessage = statusMessage;
            Body = null;
        }

        public Response(int status, string statusMessage, string body)
        {
            Status = status;
            StatusMessage = statusMessage;
            Body = body;
        }

        /*        public void Print()
                {
                    Console.WriteLine("{0} {1}", Status, StatusMessage);
                }*/

        public String PrepareAsString()
        {

            string textohnebody = "HTTP/1.1 200 OK\r\nContent-Type: plain/text\r\nContent-Length: 15\r\n";
            string textmitbody = "HTTP/1.1 200 OK\r\nContent-Type: plain/text\r\nContent-Length: 5\r\n\r\n12345";


            return "text";
        }
        public void Print()
        {
            Console.WriteLine("Response:");
            Console.WriteLine("{0} {1}\n\n{2}", Status, StatusMessage, Body);
        }


    }
}


