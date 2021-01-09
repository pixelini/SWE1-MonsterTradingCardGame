using System;
using System.Collections.Generic;
using System.Text;

namespace HttpRestServer
{
    public interface IListener
    {
        public IClient AcceptTcpClient();
        public void Start();
        public void Stop();

    }
}
