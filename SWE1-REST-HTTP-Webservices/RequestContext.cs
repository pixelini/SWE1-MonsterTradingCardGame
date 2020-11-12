using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class RequestContext
    {
        public HttpVerb Method { get; set; }
        public string ResourcePath { get; set; }
        //public int Resource { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }

        public RequestContext(string method, string resourcePath, string httpVersion, Dictionary<string, string> headers, string payload)
        {

            if (method == "GET")
            {
                Method = HttpVerb.GET;
            }

            ResourcePath = resourcePath;
            HttpVersion = httpVersion;
            Headers = headers;
            Payload = payload;
        }

        public static RequestContext ParseRequest(string data)
        {
            // get the method
            //string[] parts = data.Split(' ', '\n', '\r');
            //string[] parts = data.Split('\r','\n');
            string[] lines = data.Split('\r', '\n');


            // first line is special (lines[0]
            string firstLine = lines[0];
            string[] partsFirstLine = firstLine.Split(' ');

            string method = partsFirstLine[0]; // GET
            string resource = partsFirstLine[1]; // /messages/1
            string version = partsFirstLine[2]; // HTTP/1.1



/*            foreach (var partFirstLine in partsFirstLine)
            {
                Console.WriteLine($"<{partFirstLine}>");
            }*/


            // then all headers (starting with index 2; every even index; if empty stop)
            Dictionary<string, string> headers = new Dictionary<string, string>();

            int i = 2;
            int indexPayload = -1;
            while (lines[i] != "")
            {
                string[] splittedHeaders = lines[i].Split(':', 2);
                headers.Add(splittedHeaders[0], splittedHeaders[1]); // Exception?
                i += 2;

                if (lines[i] == "")
                {
                    indexPayload = i + 2;
                }
            }

            // get the payload in one string
            StringBuilder sb = new StringBuilder();
            while (indexPayload < lines.Length)
            {
                sb.Append(lines[indexPayload]);
                sb.Append('\n');
                indexPayload++;
            }

            string payload = sb.ToString();

            return new RequestContext(method, resource, version, headers, payload);

        }

        public void Print()
        {
            Console.WriteLine(
                "My RequestContext object:\n\n" +
                "Method: {0}\n" +
                "ResourcePath: {1}\n" +
                "HttpVersion: {2}\n\n" +
                "Payload:\n{3}\n", 
                Method, 
                ResourcePath, 
                HttpVersion, 
                Payload
                );

            Console.WriteLine("Headers: ");

            foreach (var header in Headers)
            {
                Console.WriteLine(String.Format("Key: {0, -20} Value: {1, -20} ", header.Key, header.Value)); // right aligned with -
            }


        }

        public static void HandleList()
        {

        }

        public static void HandleAdd()
        {

        }

        public static void HandleRead()
        {

        }

        public static void HandleUpdate()
        {

        }

        public static void HandleDelete()
        {

        }

    }
}
