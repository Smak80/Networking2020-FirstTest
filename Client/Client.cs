using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
                while (true)
                {
                    String userData = "";
                    userData = Console.ReadLine();
                    SendData(userData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Что-то пошло не так... :(");
            }
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

        void Communicate()
        {
            if (cSocket != null)
            {
                Console.WriteLine("Начало общения с клиентом");
                while (true)
                {
                    String d = ReceiveData();
                    Console.WriteLine("Сервер ответил: {0}", d);
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
    }
}
