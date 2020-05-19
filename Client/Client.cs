using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonNet;

namespace Client
{
    class Client
    {
        private String serverHost;
        private Socket cSocket;
        private int port = 8034;
        private NetMessaging net;
        public Client(String serverHost)
        {
            try
            {
                this.serverHost = serverHost;
                Console.WriteLine("Подключение к {0}", this.serverHost);
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(this.serverHost, port);
                net = new NetMessaging(cSocket);
                net.LoginCmdReceived += OnLogin;
                net.UserListCmdReceived += OnUserList;
                net.StartCmdReceived += OnStart;
                net.MessageCmdReceived += OnMessage;
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Не удалось получить данные. Завершение соединения...");
                    }
                }).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
        }

        private void OnMessage(string command, string data)
        {
            Console.WriteLine("{0}", data);
        }

        private void OnStart(string command, string data)
        {
            Console.WriteLine("Вы можете писать сообщения!");
            GoMessaging();
        }

        private void OnUserList(string command, string data)
        {
            var us = data.Split(',');
            Console.WriteLine("Список подключенных клиентов:");
            foreach (var cl in us)
            {
                Console.WriteLine(cl);
            }
            Console.WriteLine("-----------------------------");
        }

        private void GoMessaging()
        {
            new Thread(() =>
                {
                    while (true)
                    {
                        String userData = "";
                        userData = Console.ReadLine();
                        net.SendData("MESSAGE", userData);
                    }
                }
            ).Start();
        }

        void OnLogin(string c, string d)
        {
            String userName = "";
            Console.WriteLine("Представьтесь: ");
            userName = Console.ReadLine();
            net.SendData("LOGIN", userName);
        }

    }
}
