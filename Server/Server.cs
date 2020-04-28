using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private String host;
        private Socket sSocket;
        private const int port = 8034;
        public Server()
        {
            Console.WriteLine("Получение локального адреса сервера");
            try
            {
                host = Dns.GetHostName();
                Console.WriteLine("Имя хоста: {0}", host);
                sSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                foreach (var addr in Dns.GetHostEntry(host).AddressList)
                {
                    try
                    {
                        sSocket.Bind(
                            new IPEndPoint(addr, port)
                        );
                        Console.WriteLine("Сокет связан с: {0}:{1}", addr, port);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не удалось связать с: {0}:{1}", addr, port);
                    }
                }

                sSocket.Listen(10);
                Console.WriteLine("Прослушивание началось...");
                while (true)
                {
                    Console.WriteLine("Ожидание нового подключения...");
                    Socket cSocket = sSocket.Accept();
                    Console.WriteLine("Соединение с клиентом установлено!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }
    }
}
