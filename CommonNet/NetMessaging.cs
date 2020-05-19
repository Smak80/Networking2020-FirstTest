using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommonNet
{
    public delegate void Receiving(String command, String data);
    public class NetMessaging
    {
        private Socket cSocket;
        public event Receiving LoginCmdReceived;
        public event Receiving MessageCmdReceived;
        public event Receiving UserListCmdReceived;
        public event Receiving StartCmdReceived;
        public NetMessaging(Socket s)
        {
            cSocket = s;
        }

        public void SendData(String command, String data)
        {
            if (cSocket != null)
            {
                try
                {
                    if (data.Trim().Equals("") ||
                        command.Trim().Equals("")) return;
                    var b = Encoding.UTF8.GetBytes(command+"="+data+"\n");
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

        public String ReceiveData()
        {
            String res = "";
            if (cSocket != null)
            {
                var b = new byte[65536];
                Console.WriteLine("Ожидание данных...");
                var i = 0;
                do
                {
                    var cnt = cSocket.Receive(b);
                    Console.WriteLine("Получена порция данных №{0}", ++i);
                    var r = Encoding.UTF8.GetString(b, 0, cnt);
                    res += r;
                } while (res[res.Length-1]!='\n');
                Console.WriteLine("Данные успешно получены");
                    
            }
            return res.Trim();
        }

        public void Communicate()
        {
            if (cSocket != null)
            {
                Console.WriteLine("Начало общения...");
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
            switch (cd[0])
            {
                case "LOGIN":
                {
                    LoginCmdReceived?.Invoke(cd[0], cd[1]);
                    break;
                }
                case "MESSAGE":
                {
                    MessageCmdReceived?.Invoke(cd[0], cd[1]);
                    break;
                }
                case "USERLIST":
                {
                    UserListCmdReceived?.Invoke(cd[0], cd[1]);
                    break;
                }
                case "START":
                {
                    StartCmdReceived?.Invoke(cd[0], cd[1]);
                    break;
                }
            }
        }
    }
}
