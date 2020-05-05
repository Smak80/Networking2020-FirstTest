using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        class ConnectedClient
        {
            public Socket cSocket;
            public static List<ConnectedClient> clients = new List<ConnectedClient>();

            public ConnectedClient(Socket s)
            {
                cSocket = s;
                clients.Add(this);
                new Thread(() =>
                {
                    Communicate();
                }).Start();
            }
            void Communicate()
            {
                if (cSocket != null)
                {
                    Console.WriteLine("Начало общения с клиентом");
                    while (true)
                    {
                        String d = ReceiveData();
                        Console.WriteLine("Клиент передал: {0}", d);
                        SendData("Спасибо тебе! Твои данные получил!");
                    }
                }
            }

            String ReceiveData()
            {
                String res = "";
                if (cSocket != null)
                {
                    try
                    {
                        var b = new byte[65536];
                        Console.WriteLine("Ожидание данных от клиента...");
                        var cnt = cSocket.Receive(b);
                        Console.WriteLine("Данные от клиента успешно получены");
                        res = Encoding.UTF8.GetString(b, 0, cnt);
                    } catch (Exception ex)
                    {
                        Console.WriteLine("Не удалось получить данные от клиента :(");
                    }
                }
                return res;
            }

            public void SendData(String data)
            {
                if (cSocket != null)
                {
                    try
                    {
                        if (data.Trim().Equals("")) return;
                        var b = Encoding.UTF8.GetBytes(data);
                        Console.WriteLine("Отправка сообщения...");
                        cSocket.Send(b);
                        Console.WriteLine("Сообщение успешно отправлено!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Не удалось отправить сообщение :(");
                    }
                }
            }
        }
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
                    var cSocket = sSocket.Accept();
                    Console.WriteLine("Соединение с клиентом установлено!");
                    new ConnectedClient(cSocket);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }
    }
}
