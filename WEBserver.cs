using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    class WEBserver
    {
        IPAddress IP = IPAddress.Parse("127.0.0.1");
        int port = 8080;
        TcpListener listener;
        TcpClient client;
       
          public WEBserver()
        {
            listener = new TcpListener(IP, port);
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();
                if (client != null)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(HandlerThread));
                    thread.Start(client);
                }
            }
        }

        public void HandlerThread (object client)
        {
            new Handler((TcpClient)client);
        }

        static void Main(string[] args)
        {
            WEBserver server = new WEBserver();
        }
    }
}
