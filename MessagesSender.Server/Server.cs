using System;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace MessagesSender.Server
{
    class Server
    {
        Socket Socket;
        private MainWindow Form;
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public void Start(object form)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.Bind(ipEndPoint);
            Socket.Listen(100);
            Socket.BeginAccept(new AsyncCallback(AcceptCallback), Socket);
            Form = (MainWindow)form;
            while (!Socket.Poll(1000, SelectMode.SelectRead));
            Form.Status = "Клиент подключился";
        }
        public void AcceptCallback(IAsyncResult result)
        {
            Socket = Socket.EndAccept(result);
            Socket.BeginReceive(Buffer, 0, BufferSize, 0,
                new AsyncCallback(ReadMessages), Socket);
        }
        private void ReadMessages(IAsyncResult result)
        {
            try
            {
                int messageSize = Socket.EndReceive(result);
                string message = Encoding.UTF8.GetString(Buffer, 0, messageSize);
                Socket.BeginReceive(Buffer, 0, BufferSize, 0, new AsyncCallback(ReadMessages), Socket);
                Form.Message = message;
                SaveMessageInDatabase(message);
                byte[] response = Encoding.UTF8.GetBytes("OK");
                Socket.Send(response, response.Length, 0);
            }
            catch (SocketException)
            {
                Socket.Close();
                return;
            }
        }
        private void SaveMessageInDatabase(string message)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "insert into Messages (ID, Text, Created) values(@ID, @Text, @Created)";
                    command.Parameters.AddWithValue("@ID", Guid.NewGuid());
                    command.Parameters.AddWithValue("@Text", message);
                    command.Parameters.AddWithValue("@Created", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}