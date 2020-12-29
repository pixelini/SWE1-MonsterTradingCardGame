using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRestServer
{
    public interface IEndpointHandler
    {
        public Response HandleRequest(RequestContext req);

        public static RequestContext ParseRequest(string data)
        {
            if (data == null || data == "")
            {
                return null;
            }

            try
            {
                // get lines from whole input
                string[] lines = data.Split('\r', '\n');

                // check if client called a valid request (only first line, not the payload)
                // (with valid http method and path which is accessible for server)
                string firstLine = lines[0];
                string[] partsFirstLine = firstLine.Split(' ');
                string method = partsFirstLine[0]; // GET
                string resource = partsFirstLine[1]; // /messages/1
                string version = partsFirstLine[2]; // HTTP/1.1

                // all headers (starting with index 2; every even index; if empty stop)
                Dictionary<string, string> headers = new Dictionary<string, string>();

                int i = 2;
                int indexPayload = -1; // helps to find out where payload starts
                while (lines[i] != "")
                {
                    string[] splittedHeaders = lines[i].Split(':', 2);
                    headers.Add(splittedHeaders[0],
                        splittedHeaders[1].Trim()); // trim whitespace after value --> exception?
                    i += 2;
                }

                // check if MIME-Type is valid
                if (headers.ContainsKey("Content-Length") && !IsValidContentType(headers["Content-Type"]))
                {
                    Console.WriteLine("Can't handle specified Content-Type.");
                    return null;
                }

                string payload = null;

                if (headers.ContainsKey("Content-Length") && headers["Content-Length"] != "0")
                {
                    indexPayload = i + 2;

                    // get the payload in one string
                    StringBuilder sb = new StringBuilder();
                    while (indexPayload < lines.Length)
                    {
                        sb.Append(lines[indexPayload]);

                        if (indexPayload < lines.Length - 1)
                        {
                            //last line has already a \n
                            sb.Append('\n');
                        }

                        indexPayload++;
                    }

                    payload = sb.ToString();

                    // check if content length in header matches length of received body --> are all data transmitted correctly?
                    if (payload.Length.ToString() != headers["Content-Length"])
                    {
                        // not all data are transmitted correctly!
                        Console.WriteLine("Couldn't receive all transmitted information.");
                        return null;
                    }

                }
                else
                {
                    payload = null;
                }

                return new RequestContext(method, resource, version, headers, payload);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Request has a wrong format and couldn't be parsed" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem: " + ex.Message);
            }

            return null;

        }

        private static bool IsValidContentType(string contentType)
        {
            if (contentType == "text/plain")
            {
                return true;
            }

            Console.WriteLine("Can only handle text/plain Content-Type");
            return false;
        }

    }
}
