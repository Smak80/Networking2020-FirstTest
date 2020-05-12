using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
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
                new Thread(() =>
                {
                    Communicate();
                }).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }

        private void GoMessaging()
        {
            new Thread(() =>
                {
                    while (true)
                    {
                        String userData = "";
                        userData = Console.ReadLine();
                        SendData("MESSAGE", userData);
                    }
                }
            ).Start();
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

        void Communicate()
        {
            if (cSocket != null)
            {
                Console.WriteLine("Начало общения с сервером");
                while (true)
                {
                    String d = ReceiveData();
                    Parse(d);
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
                String userName = "";
                Console.WriteLine("Представьтесь: ");
                userName = Console.ReadLine();
                SendData("LOGIN", userName);
            } else 
            if (cd[0].ToUpper().Equals("MESSAGE"))
            {
                Console.WriteLine("{0}", cd[1]);
            } else 
            if (cd[0].ToUpper().Equals("USERLIST"))
            {
                var us = cd[1].Split(',');
                Console.WriteLine("Список подключенных клиентов:");
                foreach (var cl in us)
                {
                    Console.WriteLine(cl);
                }
                Console.WriteLine("-----------------------------");
            } else if (cd[0].ToUpper().Equals("START"))
            {
                Console.WriteLine("Вы можете писать сообщения!");
                GoMessaging();
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
    }
}
