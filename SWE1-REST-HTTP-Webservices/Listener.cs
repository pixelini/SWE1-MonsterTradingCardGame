using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SWE1_REST_HTTP_Webservices
{
    public class Listener : IListener
    {
        public TcpListener MyListener { get; set; }

        public Listener(IPAddress addr, int port)
        {
            MyListener = new TcpListener(addr, port);
        }

        public IClient AcceptTcpClient()
        {
            TcpClient connection = MyListener.AcceptTcpClient();
            Client myClient = new Client(connection);
            return myClient;
        }

        public void Start()
        {
            MyListener.Start();
        }
        public void Stop()
        {
            MyListener.Stop();
        }

    }
}
