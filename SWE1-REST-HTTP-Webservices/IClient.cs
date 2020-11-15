using System;
using System.Collections.Generic;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public interface IClient
    {
        public RequestContext ReceiveRequest();
        public void SendResponse(Response response);

    }
}
