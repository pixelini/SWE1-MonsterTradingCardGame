using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class Response
    {
        public string HttpVersion { get; set; }
        public int Status { get; set; }
        public string StatusMessage { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }


        public Response()
        {
            HttpVersion = "HTTP/1.1";
            Status = -1;
            StatusMessage = "";
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "text/plain");
            Body = null;
        }

        public Response(int status, string statusMessage)
        {
            HttpVersion = "HTTP/1.1";
            Status = status;
            StatusMessage = statusMessage;
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "text/plain");
            Headers.Add("Content-Length", "0");
            Body = null;
        }

        public Response(int status, string statusMessage, string body)
        {
            HttpVersion = "HTTP/1.1";
            Status = status;
            StatusMessage = statusMessage;
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "text/plain");
            Headers.Add("Content-Length", body.Length.ToString());
            Body = body;
        }

        public string ConvertToString()
        {
            // format of answer with or without body
            // with body, e.g. "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 0\r\n\r\n";
            // without body, e.g. "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 5\r\n\r\n12345";

            StringBuilder data = new StringBuilder();

            // add first line
            data.Append(HttpVersion);
            data.Append(" ");
            data.Append(Status);
            data.Append(" ");
            data.Append(StatusMessage);
            data.Append("\r\n");

            // add all headers
            data.Append(GetHeaders());
            data.Append("\r\n");

            // add body if necessary
            if (Body != null)
            {
                data.Append(Body);
            }
            return data.ToString();
        }

        public void Print()
        {
            Console.WriteLine("Response:");
            Console.WriteLine("{0} {1}\n\n{2}", Status, StatusMessage, Body);
        }

        private string GetHeaders()
        {
            StringBuilder allHeaders = new StringBuilder();
            foreach (var header in Headers)
            {
                allHeaders.Append(header.Key);
                allHeaders.Append(": ");
                allHeaders.Append(header.Value);
                allHeaders.Append("\r\n");
            }
            return allHeaders.ToString();
        }

        
    }
}


