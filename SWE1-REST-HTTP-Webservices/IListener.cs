using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public interface IListener
    {
        public IClient AcceptTcpClient();
        public void Start();
        public void Stop();

    }
}
