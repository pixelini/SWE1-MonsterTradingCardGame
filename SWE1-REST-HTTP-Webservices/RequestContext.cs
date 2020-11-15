using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class RequestContext
    {
        public Action Action { get; set; }
        public HttpVerb Method { get; set; }
        public string ResourcePath { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }

        public RequestContext()
        {
            Action = Action.Undefined;
            Method = HttpVerb.Get;
            ResourcePath = "";
            HttpVersion = "HTTP/1.1";
            Headers = new Dictionary<string, string>();
            Payload = null;
        }
        public RequestContext(string method, string resourcePath, string httpVersion, Dictionary<string, string> headers, string payload)
        {

            if (method == "GET")
            {
                Method = HttpVerb.Get;
            } else if (method == "PUT")
            {
                Method = HttpVerb.Put;
            } else if (method == "POST")
            {
                Method = HttpVerb.Post;
            } else if (method == "DELETE")
            {
                Method = HttpVerb.Delete;
            } else
            {
                return;
            }

            ResourcePath = resourcePath;
            HttpVersion = httpVersion;
            Headers = headers;
            Action = Action.Undefined;

            if (payload == "" || payload == "\n")
            {
                Payload = null;
            } else
            {
                Payload = payload;
            }
            
        }

        public void Print()
        {
            string payloadInfo = "";
            if (Payload == null)
            {
                payloadInfo = "not defined";
            } else
            {
                payloadInfo = Payload;
            }

            Console.WriteLine(
                "RequestContext object:\n" +
                "Method: {0}\n" +
                "ResourcePath: {1}\n" +
                "HttpVersion: {2}\n\n" +
                "Payload:\n{3}\n", 
                Method, 
                ResourcePath, 
                HttpVersion,
                payloadInfo
                );

            Console.WriteLine("Headers: ");

            foreach (var header in Headers)
            {
                Console.WriteLine($"Key: {header.Key,-20} Value: {header.Value,-20} "); // right aligned with -
            }

            Console.WriteLine();

        }

    }
}
