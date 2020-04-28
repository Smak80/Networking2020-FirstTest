using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        private String serverHost;
        private Socket cSocket;
        private int port = 8034;
        public Client(String serverHost)
        {
            try
            {
                this.serverHost = serverHost;
                Console.WriteLine("Подключение к {0}", this.serverHost);
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(this.serverHost, port);
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }
    }
}
