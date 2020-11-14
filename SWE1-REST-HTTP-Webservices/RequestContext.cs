using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    class RequestContext
    {
        public Action Action { get; set; }
        public HttpVerb Method { get; set; }
        public string ResourcePath { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Payload { get; set; }


        public RequestContext(string method, string resourcePath, string httpVersion, Dictionary<string, string> headers, string payload, Action action)
        {

            if (method == "GET")
            {
                Method = HttpVerb.GET;
            } else if (method == "PUT")
            {
                Method = HttpVerb.PUT;
            } else if (method == "POST")
            {
                Method = HttpVerb.POST;
            } else if (method == "DELETE")
            {
                Method = HttpVerb.DELETE;
            } else
            {
                // not valid! throw error
            }

            ResourcePath = resourcePath;
            HttpVersion = httpVersion;
            Headers = headers;
            Action = action;

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
            string payloadinfo = "";
            if (Payload == null)
            {
                payloadinfo = "not defined";
            } else
            {
                payloadinfo = Payload;
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
                payloadinfo
                );

            Console.WriteLine("Headers: ");

            foreach (var header in Headers)
            {
                Console.WriteLine(String.Format("Key: {0, -20} Value: {1, -20} ", header.Key, header.Value)); // right aligned with -
            }

            Console.WriteLine();

        }

    }
}
