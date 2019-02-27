using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading;

namespace MessagesSender.Client
{
    class Client
    {
        private Socket Socket;
        private ManualResetEvent ConnectionDone;
        public bool Start()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.BeginConnect(ipEndPoint, new AsyncCallback(ConnectCallback), Socket);
            ConnectionDone = new ManualResetEvent(false);
            return ConnectionDone.WaitOne(1000);
        }
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                Socket.EndConnect(result);
            }
            catch (SocketException)
            {
                return;
            }
            ConnectionDone.Set();
        }
        public string SendMessage(string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                try
                {
                    Socket.Send(buffer, buffer.Length, 0);
                }
                catch (SocketException)
                {
                    return "Клиент не подключился к серверу";
                }
                const int maxResponseSize = 1024;
                buffer = new byte[maxResponseSize];
                int responseSize = Socket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, responseSize);
                if (response != "OK")
                    return "Неизвестная ошибка на сервере";
                return null;
            }
            catch (System.IO.IOException)
            {
                return "Сервер не отвечает";
            }
        }
    }
}