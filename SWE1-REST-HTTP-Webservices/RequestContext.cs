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
        public int Resource { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }

        public RequestContext(string method, string resourcePath, string httpVersion)
        {
            if (method == "GET")
            {
                Method = HttpVerb.GET;

                // save given resourcePath in seperated variables (path and resource)

                // get path
                string dirName = System.IO.Path.GetDirectoryName(resourcePath);
                ResourcePath = dirName;

                // get resource nr of path and try to cast it in int (important that's an int because it's later used as array index)
                string msgNr = System.IO.Path.GetFileName(ResourcePath);
                int temp;
                bool successful = Int32.TryParse(msgNr, out temp);
                if (successful)
                {
                    Resource = temp;
                }
            }

            HttpVersion = httpVersion;
        }

        public static RequestContext parseRequest(string data)
        {
            //Console.WriteLine("Parsed info: \n" + data);

            // get the GET
            string[] parts = data.Split(' ', '\n', '\r');

/*            foreach (var part in parts)
            {
                Console.WriteLine($"<{part}>");
            }*/

            string method = parts[0]; // GET
            string resource = parts[1]; // /messages/1
            string version = parts[2]; // HTTP/1.1

            return new RequestContext(method, resource, version);

        }

        public void Print()
        {
            Console.WriteLine(
                "My RequestContext object:\n" +
                "Method: {0}\n" +
                "ResourcePath: {1}\n" +
                "Resource: {2}\n" +
                "HttpVersion: {3}\n" +
                "Headers: {4}\n" +
                "Payload: {5}\n", 
                Method, 
                ResourcePath, 
                Resource, 
                HttpVersion, 
                Headers, 
                Payload
                );
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
