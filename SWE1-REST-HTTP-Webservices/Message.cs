using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class Message
    {
        public string Content { get; set; }

        public string Read()
        {
            return Content;
        }

        // replaces old content with new content
        public void Update(string newContent)
        {
            Content = newContent;
        }
    }
}
