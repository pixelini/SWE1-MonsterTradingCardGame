using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRestServer
{
    public interface IClient
    {
        public RequestContext ReceiveRequest();
        public void SendResponse(Response response);

    }
}
