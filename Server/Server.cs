using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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
            public string Name { get; private set; }
            public ConnectedClient(Socket s)
            {
                cSocket = s;
                SendData("LOGIN","?");
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
                        try
                        {
                            String d = ReceiveData();
                            Console.WriteLine("Клиент передал: {0}", d);
                            Parse(d);
                        } catch (Exception ex)
                        {
                            Console.WriteLine("Не удалось получить данные от клиента :(");
                            clients.Remove(this);
                            break;
                        } 
                    }
                }
            }

            private void Parse(string s)
            {
                // КОМАНДА=ЗНАЧЕНИЕ (LOGIN=Иван)
                char[] sep = { '=' };
                var cd = s.Split(sep, 2);
                if (cd[0].ToUpper().Equals("LOGIN"))
                {
                    Name = cd[1];
                    string list = "";
                    clients.ForEach(client =>
                    {
                        list += client.Name + ",";
                    });
                    SendData("USERLIST", list);
                    clients.Add(this);
                    SendData("START", "!");
                } else 
                if (cd[0].ToUpper().Equals("MESSAGE"))
                {
                    clients.ForEach((client) =>
                    {
                        if (client != this) 
                            client.SendData("MESSAGE", Name+": "+cd[1]);
                    });
                }
            }

            String ReceiveData()
            {
                String res = "";
                if (cSocket != null)
                {
                    var b = new byte[65536];
                    Console.WriteLine("Ожидание данных от клиента...");
                    var cnt = cSocket.Receive(b);
                    Console.WriteLine("Данные от клиента успешно получены");
                    res = Encoding.UTF8.GetString(b, 0, cnt);
                }
                return res;
            }

            public void SendData(String command, String data)
            {
                if (cSocket != null)
                {
                    try
                    {
                        if (data.Trim().Equals("") ||
                            command.Trim().Equals("")) return;
                        var b = Encoding.UTF8.GetBytes(command+"="+data);
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
